# Watchdocker
Keeps an eye on specific docker images and updates appropriate containers if newer version exists

## Usages
```
docker run --name watchdocker -d \ 
    -v /var/run/docker.sock:/var/run/docker.sock \
    tgrymnak/watchdocker \
    -s=[shell] \
    -u=[username] \
    -p=[password or token] \
    -r=[registry] \
    -i=[image name] \
    -c=[container name] \
    -e=[port:port] \
    -t=[interval in seconds]
```
