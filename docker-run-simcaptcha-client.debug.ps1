docker stop simcaptcha-client-debug-container
docker rm simcaptcha-client-debug-container
docker run -d -p 5002:80 -p 5001:443 --name simcaptcha-client-debug-container simcaptcha-client-debug