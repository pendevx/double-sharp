async function handler(event) {
    var request = event.request;

    var servicePaths = [
        "/api",
    ];

    for (let i = 0; i < servicePaths.length; i++) {
        if (request.uri.startsWith(servicePaths[i])) {
            return {
                statusCode: 308,
                statusDescription: "Permanent Redirect",
                headers: {
                    location: { value: `https://gateway.music.pendevx.com${request.uri}` },
                },
            };
        }
    }
    return request;
}
