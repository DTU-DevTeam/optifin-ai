from flask import Flask, render_template, redirect, url_for, flash, jsonify, request
from flask_sqlalchemy import SQLAlchemy
from flask_wtf import FlaskForm
from wtforms import StringField, PasswordField, SubmitField
from wtforms.validators import InputRequired, Length, EqualTo
from flask_bcrypt import Bcrypt
from flask_login import LoginManager, UserMixin, login_user, login_required, logout_user, current_user
import json

app = Flask(__name__)
app.config['SQLALCHEMY_DATABASE_URI'] = 'sqlite:///users.db'
app.config['SECRET_KEY'] = 'your_secret_key'

db = SQLAlchemy(app)
bcrypt = Bcrypt(app)
login_manager = LoginManager(app)
login_manager.login_view = 'login'

DATA_FILE = 'tra_du_lieu.json'

class User(db.Model, UserMixin):
    id = db.Column(db.Integer, primary_key=True)
    username = db.Column(db.String(150), unique=True, nullable=False)
    password = db.Column(db.String(150), nullable=False)

@login_manager.user_loader
def load_user(user_id):
    return User.query.get(int(user_id))

class RegisterForm(FlaskForm):
    username = StringField('Username', validators=[InputRequired(), Length(min=4, max=150)])
    password = PasswordField('Password', validators=[InputRequired(), Length(min=6)])
    confirm_password = PasswordField('Confirm Password', validators=[InputRequired(), EqualTo('password')])
    submit = SubmitField('Register')

class LoginForm(FlaskForm):
    username = StringField('Username', validators=[InputRequired(), Length(min=4, max=150)])
    password = PasswordField('Password', validators=[InputRequired()])
    submit = SubmitField('Login')

@app.route('/register', methods=['GET', 'POST'])
def register():
    data = request.json  # Nhận dữ liệu JSON từ ASP.NET Core

    if not data or "Username" not in data or "Password" not in data:
        return jsonify({"success": False, "message": "Thiếu thông tin đăng ký"}), 400

    # Kiểm tra nếu username đã tồn tại
    existing_user = User.query.filter_by(username=data["Username"]).first()
    if existing_user:
        return jsonify({"success": False, "message": "Tên đăng nhập đã tồn tại"}), 409

    # Hash mật khẩu và lưu vào database
    hashed_password = bcrypt.generate_password_hash(data["Password"]).decode('utf-8')
    new_user = User(username=data["Username"], password=hashed_password)

    db.session.add(new_user)
    db.session.commit()

    return jsonify({"success": True, "message": "Đăng ký thành công"}), 201
    #form = RegisterForm()
    #if form.validate_on_submit():
    #    hashed_password = bcrypt.generate_password_hash(form.password.data).decode('utf-8')
    #    new_user = User(username=form.username.data, password=hashed_password)
    #    db.session.add(new_user)
    #    db.session.commit()
    #    flash('Account created successfully!', 'success')
    #    return redirect(url_for('login'))
    #return render_template('register.html', form=form)

@app.route('/login', methods=['GET', 'POST'])
def login():
    data = request.json  # Nhận dữ liệu JSON từ ASP.NET Core
    username = data.get("Username")
    password = data.get("Password")

    user = User.query.filter_by(username=username).first()
    if user and bcrypt.check_password_hash(user.password, password):
        login_user(user)
        return jsonify({"success": True, "message": "Đăng nhập thành công"}), 200
    return jsonify({"success": False, "message": "Sai tài khoản hoặc mật khẩu"}), 401

    #form = LoginForm()
    #if form.validate_on_submit():
    #    user = User.query.filter_by(username=form.username.data).first()
    #    if user and bcrypt.check_password_hash(user.password, form.password.data):
    #        login_user(user)
    #        return redirect(url_for('dashboard'))
    #    flash('Invalid username or password', 'danger')
    #return render_template('login.html', form=form)#

@app.route('/dashboard')
@login_required
def dashboard():
    dashboard_data = {'message': 'Login successful', 'username': current_user.username, 'id': current_user.id}
    try:
        with open(DATA_FILE, 'r') as file:
            data = json.load(file)
    except (FileNotFoundError, json.JSONDecodeError):
        data = []
    data.append(dashboard_data)
    with open(DATA_FILE, 'w') as file:
        json.dump(data, file, indent=4)
    return jsonify(dashboard_data), 200

@app.route('/logout')
@login_required
def logout():
    logout_user()
    return redirect(url_for('login'))

if __name__ == '__main__':
    with app.app_context():
        db.create_all()
    app.run(debug=True)
