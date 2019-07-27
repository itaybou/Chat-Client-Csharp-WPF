import datetime
import json
import time
import traceback
import uuid


class message():
	def __init__(self,user_name, message_content, message_date, group_id):
		self.user_name=user_name
		self.message_content=message_content
		self.message_date = message_date
		self.group_id = group_id
		self.guid = str(uuid.uuid4())
		self.server_time = int(time.time() * 1000)

	def __str__ (self):
		t = datetime.datetime.fromtimestamp(self.server_time/1000).strftime('%Y-%m-%d %H:%M:%S')
		return f"Group: {self.group_id}, Nickname: {self.user_name} ({t}): {self.message_content} (Message GUID: {self.guid})"

	def to_dict(self):
		try:
			return {"userName": self.user_name, "messageGuid": self.guid, "groupID": self.group_id, "msgDate": self.server_time,
			 "messageContent": self.message_content}
		except Exception as e:
			print(traceback.print_exc())
			return str(e)

	def to_json(self):
		return json.dumps(self.to_dict)