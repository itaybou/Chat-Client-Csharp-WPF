import chat_manager
from message_request import message_request
from socketserver import ThreadingMixIn
from http.server import SimpleHTTPRequestHandler
from http.server import HTTPServer
import json
import threading
import traceback

class ThreadingSimpleServer(ThreadingMixIn, HTTPServer):
	pass

class S(SimpleHTTPRequestHandler):
	def _set_headers(self):
		self.send_response(200)
		self.send_header('Content-type', 'text/html')
		self.end_headers()

	def do_GET(self):
		self._set_headers()
		self.wfile.write(bytes('<html> <head> <title>SE Intro Chat Room</title> <meta http-equiv=”refresh” content=”5" /> </head>     <body> </body> </html>', 'utf-8'))
		self.wfile.write(b'<html> <body bgcolor="#E6E6FA">')
		self.wfile.write(b"<h1> Welcome to SE Intro Chat Room 2018 </h1>")
		self.wfile.write(b"<h2> Messages: </h2>")

		for t in chat_manager.messages:
			print(t)
			self.wfile.write(bytearray("%s<br />" % str(t),encoding='utf8'))  # %str("<br>".join(realLeading)))
		self.wfile.write(b"</body></html>")

	def do_HEAD(self):
		self._set_headers()

	def do_POST(self):
		data = None
		resp = ""
		
		try:
			self._set_headers()
			self.data_string = self.rfile.read(int(self.headers['Content-Length']))
			data = json.loads(self.data_string.decode('utf-8'))
		except Exception as e: 
			print(traceback.print_exc())
			resp = str(e)
		chat_manager.semaphore.acquire()
		if data is not None:
			try:			
				req = message_request()
				resp = req.loadFromJson(data)
			except Exception as e: 
				print(traceback.print_exc())
				resp = str(e)
		chat_manager.semaphore.release()
		try:
			self.wfile.write(bytearray(resp,'utf8'))
		except Exception as e: 
			print(traceback.print_exc())


def run(server_class=HTTPServer, handler_class=S, port=80):
	global httpd
	server_address = ('', port)
	httpd = server_class(server_address, handler_class)
	print('Starting httpd...')
	threading.Thread(target=httpd.serve_forever).start()

if __name__ == "__main__":
	run(server_class=ThreadingSimpleServer)
	