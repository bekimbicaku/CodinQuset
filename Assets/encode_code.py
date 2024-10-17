import base64

def encode_user_code(code):
  encoded_code = base64.b64encode(code.encode("utf-8")).decode("utf-8")
  return encoded_code

if __name__ == "__main__":
  user_code = input("Enter code to encode: ")
  encoded_code = encode_user_code(userCode)
  print(f"Encoded code: {encoded_code}")