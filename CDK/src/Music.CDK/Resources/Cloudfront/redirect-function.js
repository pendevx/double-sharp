async function handler(event) {
    var request = event.request;

    var servicePaths = [
        "/api",
    ];

    for (let i = 0; i < servicePaths.length; i++) {
        if (request.uri.startsWith(servicePaths[i])) {
            var newUri = request.uri.slice(servicePaths[i].length)
            return {
                statusCode: 308,
                statusDescription: "Permanent Redirect",
                headers: {
                    location: { value: `https://music.cfpendevx.com${newUri}` },
                },
            };
        }
    }
    return request;
}
