from flask import Flask, send_from_directory

app = Flask(__name__)

# Trang chính - hiển thị Dashboard.html
@app.route("/")
def serve_dashboard():
    return send_from_directory(".", "Dashboard.html")

# Serve file CSS
@app.route("/Dashboard-Style.css")
def serve_css():
    return send_from_directory(".", "Dashboard-Style.css")

# Serve file JavaScript
@app.route("/Dashboard-Script.js")
def serve_js():
    return send_from_directory(".", "Dashboard-Script.js")

if __name__ == "__main__":
    app.run(debug=True, port=5000)
