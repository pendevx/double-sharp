async function handler(event) {
    var request = event.request;

    if (request.uri.startsWith("/api")) {
        return {
            statusCode: 308,
            statusDescription: "Permanent Redirect",
            headers: {
                location: { value: `http://api.music.pendevx.com${request.uri.replace(/^\/api/, "")}` },
            },
        };
    }

    return request;
}
