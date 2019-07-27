import json
import traceback
import chat_manager

class message_request:

    def recieve_message(self,request_json):
        m = [x.to_dict() for x in chat_manager.messages[-10:]]
        return json.dumps(m)

    def send_message(self,request_json):
        try:
            username = request_json["userName"]
            group_id = request_json["groupID"]
            self.user_name = username
            self.group_id = group_id
            message_content = request_json['messageContent']
            message_date = request_json["msgDate"]
            m = chat_manager.add_message(self.user_name, message_content, message_date, self.group_id)
            #return json.dumps({"messageGuid":message_guid})
            return json.dumps(m.to_dict())
        except Exception as e:
            print(traceback.print_exc())
            return str(e)

    requestTypes = {"1": send_message, "2": recieve_message}

    def loadFromJson(self, request_json):
        try:

            message_type = request_json["messageType"]
            if message_type not in self.requestTypes:
                return "Bad request type"
            self.message_type= message_type

            resp = self.requestTypes[self.message_type](self, request_json)
            return resp
        except Exception as e:
            print(traceback.print_exc())
            return str(e)