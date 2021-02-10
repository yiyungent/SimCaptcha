docker stop simcaptcha-debug-container
docker rm simcaptcha-debug-container
docker run -d -p 5004:80 -p 5003:443 -e ASPNETCORE_URLS="http://*:80" --name simcaptcha-debug-container simcaptcha-debug