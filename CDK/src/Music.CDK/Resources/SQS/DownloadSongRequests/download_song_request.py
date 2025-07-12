import json
import requests
import boto3
import io
from yt_dlp import YoutubeDL

sqs = boto3.client("sqs")
s3 = boto3.client("s3")

def handle_approved_song_requests(event, context):
    print("Received event:", json.dumps(event, indent=2))

    queue_url = event["Records"][0]["eventSourceARN"]

    for record in event.get("Records", []):
        message_id = record["messageId"]
        body = json.loads(record["body"])  # Parse the message body
        callback = body["callback"]  # Callback URL

        print(f"Processing message ID: {message_id}, Body: {body}")

        # Prepare to store the audio data in memory
        audio_data = io.BytesIO()

        def progress_hook(d):
            if d["status"] == "finished":
                # Read the file into memory
                with open(d["filename"], "rb") as f:
                    audio_data.write(f.read())
                audio_data.seek(0)  # Reset the pointer to the beginning

        ydl_opts = {
            "format": "bestaudio/best",
            "postprocessors": [{
                "key": "FFmpegExtractAudio",
                "preferredcodec": "mp3",
            }],
            "progress_hooks": [progress_hook],
            "outtmpl": "/tmp/%(title)s.%(ext)s",  # Temporary path in Lambda's /tmp directory
        }

        try:
            with YoutubeDL(ydl_opts) as ydl:
                ydl.download([body["url"]])

            s3_key = f"audio/{message_id}.mp3"  # Define the S3 key
            s3.upload_fileobj(audio_data, bucket_name, s3_key)

            print(f"Uploaded audio to S3: s3://{bucket_name}/{s3_key}")

            # Send a POST request to the callback URL
            requests.post(callback, json={"status": "processed", "messageId": message_id})

            # Delete the message from the queue
            sqs.delete_message(
                QueueUrl=queue_url,
                ReceiptHandle=record["receiptHandle"]
            )

        except Exception as e:
            print(f"Error processing message ID {message_id}: {e}")
            # Optionally handle the error (e.g., re-queue or log the failure)

    return {
        "statusCode": 200,
        "body": json.dumps({"message": "Batch processed successfully!"})
    }
