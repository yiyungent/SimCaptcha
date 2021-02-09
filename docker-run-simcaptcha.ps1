docker stop simcaptcha-container
docker rm simcaptcha-container
docker run -d -p 5004:80 -p 5003:443 -e ASPNETCORE_URLS="http://*:80" --name simcaptcha-container yiyungent/simcaptcha