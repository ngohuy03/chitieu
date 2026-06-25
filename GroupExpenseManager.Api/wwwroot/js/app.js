// app.js

// State
let currentGroupId = ""; // Should be set when a group is selected
const BASE_URL = (window.location.hostname === 'localhost' || window.location.hostname === '127.0.0.1') && (window.location.port === '5249' || window.location.port === '7159')
    ? window.location.origin
    : (window.location.origin.startsWith('file://') || window.location.hostname === 'localhost' || window.location.hostname === '127.0.0.1' ? 'http://localhost:5249' : 'https://chitieu-6tl3.onrender.com');
const apiUrl = `${BASE_URL}/api/expenses`;
let currentItems = []; // Store items for the current expense session

// Default to showing all expenses (date empty)
// document.getElementById('expenseDate').valueAsDate = new Date();

// Load data on start
document.addEventListener('DOMContentLoaded', () => {
    loadGroups();
    loadUsersForSelect();
    loadUsersForCheckboxes();
});

function loadGroups() {
    fetch(`${BASE_URL}/api/groups`)
        .then(response => response.json())
        .then(data => {
            renderGroups(data);
            if (data.length > 0) {
                currentGroupId = data[0].id;
                loadExpenses();
            } else {
                document.getElementById('expenseTableBody').innerHTML = `<tr><td colspan="5" style="text-align: center; color: var(--text-muted);">Vui lòng tạo nhóm trước.</td></tr>`;
            }
        })
        .catch(error => console.error('Error fetching groups:', error));
}

function renderGroups(groups) {
    const groupList = document.getElementById('groupList');
    if (!groups || groups.length === 0) {
        groupList.innerHTML = `<li class="category-item active">Chưa có nhóm</li>`;
        return;
    }

    groupList.innerHTML = groups.map((g, index) => `
        <li class="category-item ${index === 0 ? 'active' : ''}" onclick="selectGroup(this, '${g.id}')">
            <i class="fa-solid fa-users"></i> ${g.name}
        </li>
    `).join('');
}

function loadUsersForSelect() {
    const select = document.getElementById('paidBy');

    fetch(`${BASE_URL}/api/users`)
        .then(response => response.json())
        .then(data => {
            select.innerHTML = '<option value="">Chọn người trả</option>' +
                data.map(u => `<option value="${u.id}">${u.name}</option>`).join('');
        })
        .catch(error => console.error('Error fetching users for select:', error));
}

function loadUsersForCheckboxes() {
    const container = document.getElementById('itemParticipantsList');

    fetch(`${BASE_URL}/api/users`)
        .then(response => response.json())
        .then(data => {
            if (!data || data.length === 0) {
                container.innerHTML = '<span style="color: var(--text-muted);">Chưa có người dùng nào.</span>';
                return;
            }

            container.innerHTML = data.map(u => `
                <label class="checkbox-item">
                    <input type="checkbox" name="itemParticipants" value="${u.id}">
                    <span>${u.name}</span>
                </label>
            `).join('');
        })
        .catch(error => console.error('Error fetching users for checkboxes:', error));
}

function loadExpenses() {
    if (!currentGroupId) return;

    const date = document.getElementById('expenseDate').value;
    const tableBody = document.getElementById('expenseTableBody');

    tableBody.innerHTML = `<tr><td colspan="5" style="text-align: center; color: var(--text-muted);">Đang tải dữ liệu...</td></tr>`;

    let url = `${apiUrl}?groupId=${currentGroupId}`;
    if (date) {
        url += `&date=${date}`;
    }

    fetch(url)
        .then(response => {
            if (!response.ok) throw new Error('Network response was not ok');
            return response.json();
        })
        .then(data => {
            renderExpenses(data);
        })
        .catch(error => {
            console.error('Error fetching expenses:', error);
            tableBody.innerHTML = `<tr><td colspan="5" style="text-align: center; color: #ef4444;">Lỗi tải dữ liệu từ server!</td></tr>`;
        });
}

function renderExpenses(expenses) {
    const tableBody = document.getElementById('expenseTableBody');

    if (!expenses || expenses.length === 0) {
        tableBody.innerHTML = `<tr><td colspan="5" style="text-align: center; color: var(--text-muted);">Không có chi tiêu nào trong ngày này.</td></tr>`;
        return;
    }

    // Sắp xếp theo ngày mới nhất
    expenses.sort((a, b) => new Date(b.date) - new Date(a.date));

    tableBody.innerHTML = expenses.map(exp => {
        // Tạo chuỗi hiển thị chi tiết chia tiền
        const splitsHtml = exp.splits && exp.splits.length > 0
            ? `<div style="display: flex; flex-wrap: wrap; gap: 6px;">` + 
              exp.splits.map(s => `
                <span class="chip" style="background: rgba(255, 255, 255, 0.05); border: 1px solid rgba(255, 255, 255, 0.1); color: #e2e8f0; display: inline-flex; align-items: center; gap: 4px; padding: 4px 10px; border-radius: 12px; font-size: 0.8rem;">
                    <span style="font-weight: 500; color: #94a3b8;">${s.userName}:</span>
                    <span style="color: #38bdf8; font-weight: 600;">${formatMoney(s.owedAmount)}</span>
                </span>
              `).join('') +
              `</div>`
            : '<span style="color: var(--text-muted);">Chưa chia</span>';

        // Sử dụng toLocaleString để hiển thị cả ngày và giờ
        const dateTimeStr = new Date(exp.date).toLocaleString('vi-VN', {
            year: 'numeric',
            month: '2-digit',
            day: '2-digit',
            hour: '2-digit',
            minute: '2-digit'
        });

        let titleHtml = `<div style="font-weight: 600; color: #f8fafc;">${exp.title}</div>`;
        try {
            const items = JSON.parse(exp.title);
            if (Array.isArray(items)) {
                titleHtml = `
                    <div style="background: rgba(0,0,0,0.1); padding: 8px; border-radius: 6px;">
                        <div style="font-size: 0.8rem; color: var(--text-muted); margin-bottom: 4px;">Danh sách đồ:</div>
                        <ul style="margin: 0; padding-left: 15px; font-size: 0.85rem; color: #f8fafc;">
                            ${items.map(item => `<li>${item.name} (${formatMoney(item.price)})</li>`).join('')}
                        </ul>
                    </div>
                `;
            }
        } catch (e) {
            // Not JSON, use as is
        }

        return `
            <tr>
                <td style="padding: 16px;">
                    ${titleHtml}
                    <div style="font-size: 0.75rem; color: #64748b; margin-top: 4px; display: flex; align-items: center; gap: 4px;">
                        <i class="fa-regular fa-clock"></i> ${dateTimeStr}
                    </div>
                </td>
                <td style="padding: 16px;">
                    <span class="chip" style="background: rgba(99, 102, 241, 0.15); border: 1px solid rgba(99, 102, 241, 0.2); color: #818cf8; font-weight: 500; padding: 4px 10px; border-radius: 12px; display: inline-flex; align-items: center; gap: 4px;">
                        <i class="fa-solid fa-user" style="font-size: 0.7rem;"></i>${exp.paidByName || 'N/A'}
                    </span>
                </td>
                <td style="padding: 16px;">
                    <span style="font-weight: 700; color: #10b981; font-size: 1.1rem;">${formatMoney(exp.amount)}</span>
                </td>
                <td style="padding: 16px;">${splitsHtml}</td>
                <td style="padding: 16px;">
                    <button class="btn" style="padding: 6px 10px; font-size: 0.8rem; background: rgba(239, 68, 68, 0.1); color: #ef4444; border: 1px solid rgba(239, 68, 68, 0.2); border-radius: 8px; transition: all 0.2s;" 
                            onmouseover="this.style.background='rgba(239, 68, 68, 0.2)'" 
                            onmouseout="this.style.background='rgba(239, 68, 68, 0.1)'"
                            onclick="deleteExpense('${exp.id}')">
                        <i class="fa-solid fa-trash"></i>
                    </button>
                </td>
            </tr>
        `;
    }).join('');
}

function deleteExpense(id) {
    if (!confirm('Bạn có chắc chắn muốn xóa khoản chi này?')) return;

    fetch(`${apiUrl}/${id}`, {
        method: 'DELETE'
    })
        .then(response => {
            if (response.ok) {
                loadExpenses();
            } else {
                alert('Xóa thất bại!');
            }
        })
        .catch(error => console.error('Error deleting expense:', error));
}

function openModal() {
    document.getElementById('expenseModal').style.display = 'flex';
    // Reset form
    document.getElementById('paidBy').value = '';
    currentItems = [];
    renderItems();
    hideAddItemForm();
}

function closeModal() {
    document.getElementById('expenseModal').style.display = 'none';
}

function saveExpense() {
    const date = document.getElementById('expenseDate').value;
    const paidById = document.getElementById('paidBy').value;

    if (!paidById || currentItems.length === 0) {
        alert('Vui lòng chọn người trả và thêm ít nhất 1 món đồ!');
        return;
    }

    if (!currentGroupId) {
        alert('Vui lòng chọn nhóm!');
        return;
    }

    // Lấy giờ hiện tại để lưu cùng ngày
    const now = new Date();
    let selectedDate;
    
    if (date) {
        selectedDate = new Date(date);
        selectedDate.setHours(now.getHours(), now.getMinutes(), now.getSeconds());
    } else {
        selectedDate = new Date(); // Fallback to now if no date selected
    }

    // Lấy chuỗi thời gian theo giờ địa phương (không có chữ Z ở cuối)
    const tzoffset = selectedDate.getTimezoneOffset() * 60000;
    const localDateStr = (new Date(selectedDate - tzoffset)).toISOString().slice(0, -1);

    // Tính toán tổng tiền và chia tiền
    let totalAmount = 0;
    const userSplits = {};

    currentItems.forEach(item => {
        totalAmount += item.price;
        const share = item.price / item.participantIds.length;
        item.participantIds.forEach(userId => {
            if (!userSplits[userId]) {
                userSplits[userId] = 0;
            }
            userSplits[userId] += share;
        });
    });

    const splitsList = Object.keys(userSplits).map(userId => ({
        userId: userId,
        owedAmount: userSplits[userId]
    }));

    const dto = {
        title: JSON.stringify(currentItems),
        amount: totalAmount,
        date: localDateStr,
        groupId: currentGroupId,
        paidById: paidById,
        splits: splitsList
    };

    fetch(apiUrl, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(dto)
    })
    .then(response => {
        if (response.ok) {
            closeModal();
            loadExpenses();
        } else {
            alert('Lưu thất bại!');
        }
    })
    .catch(error => console.error('Error saving expense:', error));
}

// New functions for item management
function showAddItemForm() {
    document.getElementById('addItemForm').style.display = 'block';
    // Reset item inputs
    document.getElementById('itemName').value = '';
    document.getElementById('itemPrice').value = '';
    const checkboxes = document.querySelectorAll('input[name="itemParticipants"]');
    checkboxes.forEach(cb => cb.checked = false);
}

function hideAddItemForm() {
    document.getElementById('addItemForm').style.display = 'none';
}

function addItem() {
    const name = document.getElementById('itemName').value;
    const price = parseFloat(document.getElementById('itemPrice').value);
    const checkboxes = document.querySelectorAll('input[name="itemParticipants"]:checked');
    const participantIds = Array.from(checkboxes).map(cb => cb.value);

    if (!name || !price || participantIds.length === 0) {
        alert('Vui lòng nhập đầy đủ tên đồ, giá và chọn ít nhất 1 người tham gia!');
        return;
    }

    currentItems.push({
        name: name,
        price: price,
        participantIds: participantIds
    });

    renderItems();
    hideAddItemForm();
}

function renderItems() {
    const container = document.getElementById('itemsList');
    const totalSpan = document.getElementById('totalAmount');

    if (currentItems.length === 0) {
        container.innerHTML = '<span style="color: var(--text-muted);">Chưa có món nào được thêm.</span>';
        totalSpan.textContent = '0 đ';
        return;
    }

    let total = 0;
    container.innerHTML = currentItems.map((item, index) => {
        total += item.price;
        return `
            <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 5px; padding: 5px; background: rgba(255,255,255,0.02); border-radius: 4px;">
                <div>
                    <div style="font-weight: 600;">${item.name}</div>
                    <div style="font-size: 0.8rem; color: var(--text-muted);">Số người: ${item.participantIds.length}</div>
                </div>
                <div style="display: flex; align-items: center; gap: 10px;">
                    <span style="color: #10b981; font-weight: 600;">${formatMoney(item.price)}</span>
                    <button class="btn" style="padding: 2px 6px; font-size: 0.8rem; background: #ef4444;" onclick="removeItem(${index})">
                        <i class="fa-solid fa-times"></i>
                    </button>
                </div>
            </div>
        `;
    }).join('');

    totalSpan.textContent = formatMoney(total);
}

function removeItem(index) {
    currentItems.splice(index, 1);
    renderItems();
}

function selectAllItemParticipants() {
    const checkboxes = document.querySelectorAll('input[name="itemParticipants"]');
    checkboxes.forEach(cb => cb.checked = true);
}

function selectGroup(element, groupId) {
    document.querySelectorAll('.category-item').forEach(el => el.classList.remove('active'));
    element.classList.add('active');
    currentGroupId = groupId;
    loadExpenses();
}

function formatMoney(amount) {
    return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(amount);
}

function selectAllParticipants() {
    const checkboxes = document.querySelectorAll('input[name="participants"]');
    checkboxes.forEach(cb => cb.checked = true);
}
