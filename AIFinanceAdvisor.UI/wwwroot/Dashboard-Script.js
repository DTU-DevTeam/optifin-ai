// Lưu ý: Các hàm trong file này chỉ dùng cho trang Dashboard.html
// Ẩn màn hình chờ khi trang đã tải xong
window.onload = function () {
    setTimeout(function () {
        let loadingScreen = document.getElementById("loading-screen");
        if (loadingScreen) {
            loadingScreen.style.display = "none";
        }
        document.getElementById("loading-screen").style.display = "none";
    }, 3000);
};

// Menu Tab
document.addEventListener("DOMContentLoaded", function () {
    const navLinks = document.querySelectorAll(".navbar a");

    navLinks.forEach((link) => {
        link.addEventListener("click", function () {
            navLinks.forEach((nav) => nav.classList.remove("active"));
            this.classList.add("active");
        });
    });
});

// USERS INFORMATION //
let isLoggedIn = false;
let userName = "Profile Name";
let userAvatar =
    "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQmRLRMXynnc7D6-xfdpeaoEUeon2FaU0XtPg&s";
let typingTimeout;
// USERS INFORMATION //

// LOGIN FORM //
document.addEventListener("DOMContentLoaded", function () {
    let loginButton = document.getElementById("login-btn");
    let loginModal = document.getElementById("login-modal");
    let closeModal = document.getElementById("close-login");

    loginButton.addEventListener("click", function () {
        loginModal.classList.remove("hide");
        loginModal.classList.add("show");
        loginModal.style.display = "block";
    });

    closeModal.addEventListener("click", function () {
        loginModal.classList.remove("show");
        loginModal.classList.add("hide");

        setTimeout(() => {
            loginModal.style.display = "none";
        }, 400);
    });
});
// LOGIN FORM //

// STATUS OF USERS CHECKING //
function checkLoginStatus() {
    if (!isLoggedIn) {
        document.getElementById("user-info").style.display = "flex"; // Hiển thị form nhập tên và email
    }
}
// STATUS OF USERS CHECKING //

// Lưu thông tin người dùng khi đăng nhập
function saveUserInfo() {
    userName = document.getElementById("user-name").value;
    const userEmail = document.getElementById("user-email").value;
    if (userName && userEmail) {
        isLoggedIn = true;
        document.getElementById("user-info").style.display = "none"; // Ẩn form sau khi nhập
        sendMessage();
    }
}

function toggleChatBox() {
    const chatBox = document.getElementById("chat-box");
    if (chatBox.style.display === "none" || chatBox.style.display === "") {
        chatBox.style.display = "flex";
    } else {
        chatBox.style.display = "none";
    }
}

function sendMessage() {
    const input = document.getElementById("chat-input");
    const messages = document.getElementById("chat-messages");
    const message = input.value.trim();
    if (message) {
        const messageElement = document.createElement("div");
        messageElement.classList.add("chat-message");

        const avatarElement = document.createElement("img");
        avatarElement.classList.add("avatar");
        avatarElement.src = userAvatar;

        const contentElement = document.createElement("div");
        contentElement.classList.add("message-content");

        const usernameElement = document.createElement("div");
        usernameElement.classList.add("username");
        usernameElement.textContent = userName;

        contentElement.appendChild(usernameElement);
        contentElement.appendChild(document.createTextNode(message));

        messageElement.appendChild(avatarElement);
        messageElement.appendChild(contentElement);

        messages.appendChild(messageElement);
        input.value = "";
        messages.scrollTop = messages.scrollHeight;

        // Ẩn câu chào mừng sau khi người dùng gửi tin nhắn đầu tiên
        const welcomeMessage = document.querySelector(".welcome-message");
        if (welcomeMessage) {
            welcomeMessage.style.display = "none";
        }
    }
}

document.getElementById("chat-input").addEventListener("keydown", function (e) {
    if (e.key === "Enter") {
        e.preventDefault();
        sendMessage();
    }
});