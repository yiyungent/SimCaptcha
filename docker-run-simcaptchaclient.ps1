docker stop simcaptchaclient-container
docker rm simcaptchaclient-container
docker run -d -p 5002:80 -p 5001:443 --name simcaptchaclient-container yiyungent/simcaptchaclient