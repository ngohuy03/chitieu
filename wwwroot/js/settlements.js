// settlements.js

document.addEventListener('DOMContentLoaded', () => {
    loadGroupsForSelect();
});

function loadGroupsForSelect() {
    const select = document.getElementById('groupSelect');
    
    fetch('/api/groups')
        .then(response => response.json())
        .then(data => {
            select.innerHTML = '<option value="">Chọn Nhóm</option>' + 
                data.map(g => `<option value="${g.id}">${g.name}</option>`).join('');
            
            if (data.length > 0) {
                select.value = data[0].id;
                loadSettlements();
            }
        })
        .catch(error => console.error('Error fetching groups:', error));
}

function loadSettlements() {
    const groupId = document.getElementById('groupSelect').value;
    const listContainer = document.getElementById('settlementList');
    
    if (!groupId) {
        listContainer.innerHTML = `<div style="text-align: center; color: var(--text-muted); padding: 20px;">Vui lòng chọn nhóm để xem quyết toán.</div>`;
        return;
    }

    listContainer.innerHTML = `<div style="text-align: center; color: var(--text-muted); padding: 20px;">Đang tính toán...</div>`;

    fetch(`/api/expenses/settlements?groupId=${groupId}`)
        .then(response => response.json())
        .then(data => {
            renderSettlements(data);
        })
        .catch(error => {
            console.error('Error fetching settlements:', error);
            listContainer.innerHTML = `<div style="text-align: center; color: #ef4444; padding: 20px;">Lỗi tính toán quyết toán!</div>`;
        });
}

function renderSettlements(settlements) {
    const listContainer = document.getElementById('settlementList');
    
    if (!settlements || settlements.length === 0) {
        listContainer.innerHTML = `
            <div style="text-align: center; color: #10b981; padding: 30px;">
                <i class="fa-solid fa-circle-check" style="font-size: 2rem; margin-bottom: 10px;"></i>
                <div>Tuyệt vời! Tất cả các khoản nợ đã được tất toán xong.</div>
            </div>
        `;
        return;
    }

    listContainer.innerHTML = settlements.map(s => `
        <div class="settlement-item">
            <div class="user-names">
                <span class="debtor">${s.fromUserName}</span>
                <i class="fa-solid fa-arrow-right" style="color: var(--text-muted);"></i>
                <span class="creditor">${s.toUserName}</span>
            </div>
            <div class="amount">${formatMoney(s.amount)}</div>
        </div>
    `).join('');
}

function formatMoney(amount) {
    return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(amount);
}
