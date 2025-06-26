import socket
import time
import os

host = "127.0.0.1"
port = 41912

max_tries = 5
time_between_tries = 100
tries = 0

s = socket.socket()

while tries < max_tries:
    try:
        s.connect((host, port))
        print("Debug server is up and running")
        os._exit(0)
    except:
        print("Debug server is not up yet, retrying...")
        tries = tries + 1
        time.sleep(time_between_tries / 1000)

print("Timed out while waiting for debug server to be up")
os._exit(1)
