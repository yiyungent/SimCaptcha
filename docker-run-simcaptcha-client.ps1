docker stop simcaptcha-client-container
docker rm simcaptcha-client-container
docker run -d -p 5002:80 -p 5001:443 --name simcaptcha-client-container yiyungent/simcaptcha-client