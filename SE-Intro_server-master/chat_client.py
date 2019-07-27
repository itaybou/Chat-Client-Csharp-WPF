import json
import string
import urllib
import sys
import time
import random
import traceback
import urllib.parse
import urllib.request

def make_req(data):
    url = 'http://127.0.0.1:80'
    data = json.dumps(data)
    data = data.encode('utf8')  # data should be bytes
    req = urllib.request.Request(url, data)
    with urllib.request.urlopen(req) as response:
        response_data = response.read()
    return response_data

def send_message(user_name,gid, message_text):
    req = {"messageGuid":"null","userName": user_name, "groupID":gid, "messageContent":message_text, "messageType":"1", "msgDate": int(time.time()*1000) }
    return make_req(req)

def get_messages():
    req = { "messageType": "2" }
    return make_req(req)

def random_string(N):
    return ''.join(random.choice(string.ascii_uppercase + string.digits) for _ in range(N))

def send_messages_routine():
    user_name = random_string(7)
    gid = random_string(3)
    while True:
        try:
            print('response: ',json.loads(send_message(user_name,gid,random_string(random.randint(1,10)))))
        except Exception as e:
            print(traceback.print_exc())
            exit()
        time.sleep(random.randint(1,10))
def get_messages_routine():
    while True:
        try:
            print('response: ', json.loads(get_messages()))
        except Exception as e:
            print(traceback.print_exc())
            exit()
        time.sleep(random.randint(1, 10))

print(sys.argv[1])
if sys.argv[1]=='send':
    send_messages_routine()
elif sys.argv[1]=='get':
    get_messages_routine()
else:
    print('first argument must be "send" or "get"')
    exit()