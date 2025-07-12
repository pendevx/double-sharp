# Use the official AWS Lambda Python base image
FROM public.ecr.aws/lambda/python:3.13

# Copy the function code and requirements file
COPY download_song_request.py ${LAMBDA_TASK_ROOT}/
COPY requirements.txt ${LAMBDA_TASK_ROOT}/

# Install dependencies
RUN pip install -r requirements.txt --target "${LAMBDA_TASK_ROOT}"

# Set the CMD to your handler
CMD ["download_song_request.handle_approved_song_requests"]
