console.log('Script js/product.js DEFINIDO.');

const API_BASE_URL = 'http://localhost:5087/api';
const originalRowHTML = {};

// =======================================================
// LÓGICA DO FORMULÁRIO DE CADASTRO
// =======================================================
async function loadProductCategories(selectElement, defaultOptionText = 'Selecione uma categoria') {
    if (!selectElement) return;
    try {
        const accessToken = localStorage.getItem('accessToken');
        const response = await fetch(`${API_BASE_URL}/categories`, { headers: { 'Authorization': `Bearer ${accessToken}` } });
        const categories = await response.json();
        selectElement.innerHTML = `<option value="">${defaultOptionText}</option>`;
        categories.forEach(category => {
            const option = new Option(category.name, category.id);
            selectElement.appendChild(option);
        });
    } catch (error) {
        console.error('Erro ao carregar categorias:', error);
        selectElement.innerHTML = '<option value="">Erro ao carregar</option>';
    }
}

function initializeProductForm(form) {
    if (!form) return;
    form.addEventListener('submit', (event) => {
        event.preventDefault();
        processAndSendProductData(form);
    });
}

async function processAndSendProductData(form) {
    const formData = new FormData(form);
    console.log('%c--- DEBUG: DADOS CADASTRO ---', 'color: blue; font-weight: bold;');
    for (const [key, value] of formData.entries()) { console.log(`➡️ ${key}: "${value}"`); }
    
    if (!formData.get('Code')?.trim() || !formData.get('Name')?.trim() || !formData.get('CategoryId')) {
        alert('Por favor, preencha os campos obrigatórios: Código, Nome e Categoria.');
        return;
    }
    try {
        const accessToken = localStorage.getItem('accessToken');
        const response = await fetch(`${API_BASE_URL}/products`, {
            method: 'POST',
            headers: { 'Authorization': `Bearer ${accessToken}` },
            body: formData,
        });
        if (response.ok) {
            alert('Produto cadastrado com sucesso!');
            form.reset();
            fetchAndRenderProducts();
        } else {
            const errorData = await response.json();
            alert(`Erro: ${errorData.title || errorData.message || 'Erro ao salvar o produto.'}`);
        }
    } catch (error) {
        console.error('❌ Erro na requisição:', error);
        alert('Falha na comunicação com o servidor.');
    }
}


// =======================================================
// LÓGICA DA TABELA (COM CRUD COMPLETO E NOMES CORRIGIDOS)
// =======================================================

async function fetchAndRenderProducts() {
    const tableBody = document.querySelector('#product-list-body');
    if (!tableBody) return;
    tableBody.innerHTML = '<tr><td colspan="8" style="text-align: center;">Buscando produtos...</td></tr>';
    try {
        const accessToken = localStorage.getItem('accessToken');
        const response = await fetch(`${API_BASE_URL}/products`, { headers: { 'Authorization': `Bearer ${accessToken}` } });
        const products = await response.json();
        renderProductTable(products);
    } catch (error) {
        console.error("❌ Erro ao buscar produtos:", error);
        tableBody.innerHTML = `<tr><td colspan="8" style="text-align: center; color: red;">${error.message}</td></tr>`;
    }
}

function renderProductTable(products) {
    const tableBody = document.querySelector('#product-list-body');
    tableBody.innerHTML = '';
    if (!products || products.length === 0) {
        tableBody.innerHTML = '<tr><td colspan="8" style="text-align: center;">Nenhum produto cadastrado.</td></tr>';
        return;
    }
    products.forEach(product => {
        const imageUrl = product.imageUrl || 'https://via.placeholder.com/60';
        const productJsonString = JSON.stringify(product).replace(/'/g, "&apos;");
        const formattedValue = (product.value || 0).toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
        
        const rowHTML = `
            <tr id="row-${product.id}">
                <td><img src="${imageUrl}" alt="${product.name}" class="product-table-img"></td>
                <td data-field="code">${product.code || 'N/A'}</td>
                <td data-field="name">${product.name}</td>
                <td data-field="category" data-category-id="${product.categoryId}">${product.categoryName || 'N/A'}</td>
                <td data-field="stock">${product.stockCurrent || 0}</td>
                <td data-field="minStock">${product.stockMinium || 0}</td>
                <td data-field="value">${formattedValue}</td>
                <td class="actions-cell" data-field="actions">
                    <button class="btn-action btn-edit" onclick='editProduct(${productJsonString})'>Editar</button>
                    <button class="btn-action btn-delete" onclick="deleteProduct('${product.id}')">Excluir</button>
                </td>
            </tr>
        `;
        tableBody.insertAdjacentHTML('beforeend', rowHTML);
    });
}

window.deleteProduct = async (productId) => {
    if (!confirm('Tem certeza que deseja excluir este produto?')) return;
    try {
        const accessToken = localStorage.getItem('accessToken');
        const response = await fetch(`${API_BASE_URL}/products/${productId}`, { method: 'DELETE', headers: { 'Authorization': `Bearer ${accessToken}` } });
        if (response.ok) {
            alert('Produto excluído com sucesso!');
            fetchAndRenderProducts();
        } else {
            throw new Error('Falha ao excluir o produto.');
        }
    } catch (error) {
        alert(error.message);
    }
};

window.editProduct = async (product) => {
    const row = document.getElementById(`row-${product.id}`);
    if (!row) return;
    originalRowHTML[product.id] = row.innerHTML;
    
    // Usando os nomes de campo corretos da API
    row.querySelector('[data-field="code"]').innerHTML = `<input type="text" name="Code" class="edit-input" value="${product.code || ''}">`;
    row.querySelector('[data-field="name"]').innerHTML = `<input type="text" name="Name" class="edit-input" value="${product.name}">`;
    row.querySelector('[data-field="minStock"]').innerHTML = `<input type="number" name="StockMinium" class="edit-input" value="${product.stockMinium || 0}">`;
    row.querySelector('[data-field="value"]').innerHTML = `<input type="number" step="0.01" name="Value" class="edit-input" value="${product.value || 0}">`;

    const categoryCell = row.querySelector('[data-field="category"]');
    const categorySelect = document.createElement('select');
    categorySelect.className = 'edit-input';
    categorySelect.name = 'CategoryId';
    categoryCell.innerHTML = '';
    categoryCell.appendChild(categorySelect);
    await loadProductCategories(categorySelect);
    categorySelect.value = product.categoryId;

    row.querySelector('[data-field="actions"]').innerHTML = `
        <button class="btn-action btn-save" onclick="saveProductChanges('${product.id}')">Salvar</button>
        <button class="btn-action btn-cancel" onclick="cancelEdit('${product.id}')">Cancelar</button>
    `;
};

window.saveProductChanges = async (productId) => {
    const row = document.getElementById(`row-${productId}`);
    if (!row) return;
    const formData = new FormData();
    formData.append('Id', productId);
    const inputs = row.querySelectorAll('input, select');
    inputs.forEach(input => {
        if (input.name === 'Value') {
            formData.append(input.name, input.value.replace(',', '.'));
        } else {
            formData.append(input.name, input.value);
        }
    });

    console.log('%c--- DEBUG (ATUALIZAÇÃO) ---', 'color: green; font-weight: bold;');
    for (const [key, value] of formData.entries()) { console.log(`➡️ ${key}: "${value}"`); }

    try {
        const accessToken = localStorage.getItem('accessToken');
        const response = await fetch(`${API_BASE_URL}/products`, {
            method: 'PUT',
            headers: { 'Authorization': `Bearer ${accessToken}` },
            body: formData,
        });
        if (response.ok) {
            alert('Produto atualizado com sucesso!');
            fetchAndRenderProducts();
        } else {
            const errorData = await response.json();
            throw new Error(errorData.message || 'Falha ao atualizar o produto.');
        }
    } catch (error) {
        alert(error.message);
        cancelEdit(productId);
    }
};

window.cancelEdit = (productId) => {
    const row = document.getElementById(`row-${productId}`);
    if (row && originalRowHTML[productId]) {
        row.innerHTML = originalRowHTML[productId];
        delete originalRowHTML[productId];
    }
};

// =======================================================
// INICIALIZAÇÃO DA PÁGINA
// =======================================================

function initDynamicForm() {
    console.log('▶️ initDynamicForm() de product.js foi chamada.');
    const formElement = document.querySelector('.product-form');
    initializeProductForm(formElement);
    loadProductCategories(document.querySelector('select[name="CategoryId"]'));
    fetchAndRenderProducts();
}