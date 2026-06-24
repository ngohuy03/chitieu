// users.js

const BASE_URL = "https://chitieu-6tl3.onrender.com";
const apiUrl = `${BASE_URL}/api/users`;

document.addEventListener('DOMContentLoaded', () => {
    loadUsers();
});

function loadUsers() {
    const tableBody = document.getElementById('userTableBody');
    tableBody.innerHTML = `<tr><td colspan="3" style="text-align: center; color: var(--text-muted);">Đang tải dữ liệu...</td></tr>`;

    fetch(apiUrl)
        .then(response => response.json())
        .then(data => {
            renderUsers(data);
        })
        .catch(error => {
            console.error('Error fetching users:', error);
            tableBody.innerHTML = `<tr><td colspan="3" style="text-align: center; color: #ef4444;">Lỗi tải dữ liệu!</td></tr>`;
        });
}

function renderUsers(users) {
    const tableBody = document.getElementById('userTableBody');
    
    if (!users || users.length === 0) {
        tableBody.innerHTML = `<tr><td colspan="3" style="text-align: center; color: var(--text-muted);">Chưa có người dùng nào.</td></tr>`;
        return;
    }

    tableBody.innerHTML = users.map(user => `
        <tr>
            <td>${user.name}</td>
            <td>${user.phoneNumber || ''}</td>
            <td>
                <button class="btn" style="padding: 5px 10px; font-size: 0.8rem; background: #fb923c;" onclick="openEditModal('${user.id}', '${user.name}', '${user.phoneNumber || ''}')">
                    <i class="fa-solid fa-pen"></i>
                </button>
                <button class="btn" style="padding: 5px 10px; font-size: 0.8rem; background: #ef4444;" onclick="deleteUser('${user.id}')">
                    <i class="fa-solid fa-trash"></i>
                </button>
            </td>
        </tr>
    `).join('');
}

function openModal() {
    document.getElementById('modalTitle').innerText = "Thêm Người Dùng Mới";
    document.getElementById('userId').value = "";
    document.getElementById('userName').value = "";
    document.getElementById('userPhone').value = "";
    document.getElementById('userModal').style.display = 'flex';
}

function openEditModal(id, name, phoneNumber) {
    document.getElementById('modalTitle').innerText = "Chỉnh Sửa Người Dùng";
    document.getElementById('userId').value = id;
    document.getElementById('userName').value = name;
    document.getElementById('userPhone').value = phoneNumber || "";
    document.getElementById('userModal').style.display = 'flex';
}

function closeModal() {
    document.getElementById('userModal').style.display = 'none';
}

function saveUser() {
    const id = document.getElementById('userId').value;
    const name = document.getElementById('userName').value;
    const phoneNumber = document.getElementById('userPhone').value;

    if (!name) {
        alert('Vui lòng nhập tên!');
        return;
    }

    const dto = { name, phoneNumber };

    const method = id ? 'PUT' : 'POST';
    const url = id ? `${apiUrl}/${id}` : apiUrl;

    fetch(url, {
        method: method,
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(dto)
    })
    .then(response => {
        if (response.ok) {
            closeModal();
            loadUsers();
        } else {
            alert('Lưu thất bại!');
        }
    })
    .catch(error => console.error('Error saving user:', error));
}

function deleteUser(id) {
    if (!confirm('Bạn có chắc chắn muốn xóa người dùng này?')) return;

    fetch(`${apiUrl}/${id}`, {
        method: 'DELETE'
    })
    .then(response => {
        if (response.ok) {
            loadUsers();
        } else {
            alert('Xóa thất bại!');
        }
    })
    .catch(error => console.error('Error deleting user:', error));
}
