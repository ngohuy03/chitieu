// groups.js

const BASE_URL = window.location.origin;
const apiUrl = `${BASE_URL}/api/groups`;

document.addEventListener('DOMContentLoaded', () => {
    loadGroups();
    loadUsersForSelect();
});

function loadGroups() {
    const search = document.getElementById('searchInput').value;
    const tableBody = document.getElementById('groupTableBody');
    tableBody.innerHTML = `<tr><td colspan="5" style="text-align: center; color: var(--text-muted);">Đang tải dữ liệu...</td></tr>`;

    let url = apiUrl;
    if (search) {
        url += `?search=${encodeURIComponent(search)}`;
    }

    fetch(url)
        .then(response => response.json())
        .then(data => {
            renderGroups(data);
        })
        .catch(error => {
            console.error('Error fetching groups:', error);
            tableBody.innerHTML = `<tr><td colspan="5" style="text-align: center; color: #ef4444;">Lỗi tải dữ liệu!</td></tr>`;
        });
}

function renderGroups(groups) {
    const tableBody = document.getElementById('groupTableBody');
    
    if (!groups || groups.length === 0) {
        tableBody.innerHTML = `<tr><td colspan="5" style="text-align: center; color: var(--text-muted);">Chưa có nhóm nào.</td></tr>`;
        return;
    }

    tableBody.innerHTML = groups.map(group => `
        <tr>
            <td>${group.code}</td>
            <td>${group.name}</td>
            <td><span class="amount">${formatMoney(group.amount)}</span></td>
            <td><span class="chip">${group.contributorName || 'N/A'}</span></td>
            <td>
                <button class="btn" style="padding: 5px 10px; font-size: 0.8rem; background: #fb923c;" 
                    onclick="openEditModal('${group.id}', '${group.code}', '${group.name}', '${group.description}', ${group.amount}, '${group.contributorId}')">
                    <i class="fa-solid fa-pen"></i>
                </button>
                <button class="btn" style="padding: 5px 10px; font-size: 0.8rem; background: #ef4444;" onclick="deleteGroup('${group.id}')">
                    <i class="fa-solid fa-trash"></i>
                </button>
            </td>
        </tr>
    `).join('');
}

function loadUsersForSelect() {
    const select = document.getElementById('contributorId');
    
    fetch(`${BASE_URL}/api/users`)
        .then(response => response.json())
        .then(data => {
            select.innerHTML = '<option value="">Chọn người đóng góp</option>' + 
                data.map(u => `<option value="${u.id}">${u.name}</option>`).join('');
        })
        .catch(error => console.error('Error fetching users:', error));
}

function openModal() {
    document.getElementById('modalTitle').innerText = "Thêm Nhóm Mới";
    document.getElementById('groupId').value = "";
    document.getElementById('groupCode').value = "";
    document.getElementById('groupName').value = "";
    document.getElementById('groupDescription').value = "";
    document.getElementById('groupAmount').value = "";
    document.getElementById('contributorId').value = "";
    document.getElementById('groupModal').style.display = 'flex';
}

function openEditModal(id, code, name, description, amount, contributorId) {
    document.getElementById('modalTitle').innerText = "Chỉnh Sửa Nhóm";
    document.getElementById('groupId').value = id;
    document.getElementById('groupCode').value = code;
    document.getElementById('groupName').value = name;
    document.getElementById('groupDescription').value = description;
    document.getElementById('groupAmount').value = amount;
    document.getElementById('contributorId').value = contributorId || "";
    document.getElementById('groupModal').style.display = 'flex';
}

function closeModal() {
    document.getElementById('groupModal').style.display = 'none';
}

function saveGroup() {
    const id = document.getElementById('groupId').value;
    const code = document.getElementById('groupCode').value;
    const name = document.getElementById('groupName').value;
    const description = document.getElementById('groupDescription').value;
    const amount = parseFloat(document.getElementById('groupAmount').value);
    const contributorId = document.getElementById('contributorId').value;

    if (!code || !name || isNaN(amount)) {
        alert('Vui lòng nhập đầy đủ thông tin!');
        return;
    }

    const dto = {
        code,
        name,
        description,
        amount,
        contributorId: contributorId || null
    };

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
            loadGroups();
        } else {
            alert('Lưu thất bại!');
        }
    })
    .catch(error => console.error('Error saving group:', error));
}

function deleteGroup(id) {
    if (!confirm('Bạn có chắc chắn muốn xóa nhóm này?')) return;

    fetch(`${apiUrl}/${id}`, {
        method: 'DELETE'
    })
    .then(response => {
        if (response.ok) {
            loadGroups();
        } else {
            alert('Xóa thất bại!');
        }
    })
    .catch(error => console.error('Error deleting group:', error));
}

function formatMoney(amount) {
    return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(amount);
}
