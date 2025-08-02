console.log('Script js/lancamento.js DEFINIDO.');

// =======================================================
// INICIALIZAÇÃO
// =======================================================
function initDynamicForm() {
    console.log('▶️ initDynamicForm() de lancamento.js foi chamada.');
    initializeLaunchForm();
}

function initializeLaunchForm() {
    const typeSelection = document.getElementById('type-selection-group');
    const launchForm = document.getElementById('launchForm');
    const statusSelect = document.getElementById('status');

    // Popula os menus <select>
    populateEnumSelects();
    loadProductCategories(document.getElementById('categoryId'), 'Selecione uma categoria');

    // Listener para a escolha inicial (Entrada ou Saída)
    typeSelection.addEventListener('change', (event) => {
        const typeValue = event.target.value;
        updateFormVisibility(typeValue);
    });

    // Listener para o status (mostra/esconde data de vencimento)
    statusSelect.addEventListener('change', (event) => {
        const statusValue = event.target.value;
        const dueDateGroup = document.getElementById('group-dueDate');
        // Status "Pending" é 0
        dueDateGroup.style.display = (statusValue === '0') ? 'block' : 'none';
    });

    // Listener para o submit do formulário
    launchForm.addEventListener('submit', handleLaunchSubmit);
}

function updateFormVisibility(type) {
    const launchForm = document.getElementById('launchForm');
    const categoryGroup = document.getElementById('group-categoryId');
    const customerGroup = document.getElementById('group-customerId');

    launchForm.style.display = 'block';

    // Se for Entrada (Income = 1)
    if (type === '1') {
        categoryGroup.style.display = 'none';
        customerGroup.style.display = 'block';
    } 
    // Se for Saída (Expense = 2)
    else if (type === '2') {
        categoryGroup.style.display = 'block';
        customerGroup.style.display = 'none';
    }
}

function populateEnumSelects() {
    // Mapeamento dos enums do backend
    const paymentMethodMap = { 0: 'Cash', 1: 'CXPJ', 2: 'BBJ', 3: 'BBJS', 4: 'CHECK' };
    const statusMap = { 0: 'Pendente', 1: 'Pago' };

    const paymentSelect = document.getElementById('paymentMethod');
    const statusSelect = document.getElementById('status');

    for (const [key, value] of Object.entries(paymentMethodMap)) {
        paymentSelect.appendChild(new Option(value, key));
    }
    for (const [key, value] of Object.entries(statusMap)) {
        statusSelect.appendChild(new Option(value, key));
    }
}

async function handleLaunchSubmit(event) {
    event.preventDefault();
    const form = event.target;
    const formData = new FormData(form);
    
    // Constrói o objeto payload para enviar como JSON
    const payload = {};
    formData.forEach((value, key) => {
        // Converte campos numéricos
        if (key === 'Amount' || key === 'Type' || key === 'PaymentMethod' || key === 'Status') {
            payload[key] = parseFloat(value);
        } else if (value) { // Inclui o campo apenas se tiver valor
            payload[key] = value;
        }
    });

    // Adiciona o tipo (Entrada/Saída) que não está no formulário visível
    const selectedType = document.querySelector('input[name="Type"]:checked').value;
    payload['Type'] = parseInt(selectedType);

    try {
        const accessToken = localStorage.getItem('accessToken');
        const response = await fetch(`${API_BASE_URL}/launch`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${accessToken}`
            },
            body: JSON.stringify(payload)
        });

        if (response.ok) {
            alert('Lançamento salvo com sucesso!');
            form.reset();
            document.getElementById('type-selection-group').querySelector('input:checked').checked = false;
            launchForm.style.display = 'none';
        } else {
            const errorData = await response.json();
            showErrorModal(errorData);
        }
    } catch (error) {
        showErrorModal({ title: "Erro de Conexão", detail: error.message });
    }
}