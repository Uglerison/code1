if (!localStorage.getItem('token')) {
    window.location.href = '/login.html';
}

document.addEventListener('DOMContentLoaded', () => {
    const API_URL = 'http://localhost:5000/api';
    let expenses = [];
    
    // Elementos do DOM
    const expenseForm = document.getElementById('expenseForm');
    const searchInput = document.getElementById('searchExpense');
    const filterCategory = document.getElementById('filterCategory');
    const totalExpenses = document.getElementById('totalExpenses');
    
    // Carregar gastos iniciais
    loadExpenses();
    
    // Event Listeners
    expenseForm.addEventListener('submit', handleSubmit);
    searchInput.addEventListener('input', filterExpenses);
    filterCategory.addEventListener('change', filterExpenses);
    
    async function loadExpenses() {
        try {
            const response = await fetch(`${API_URL}/expenses`, {
                headers: {
                    'Authorization': `Bearer ${localStorage.getItem('token')}`
                }
            });
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            expenses = await response.json();
            updateExpensesList();
            updateTotalExpenses();
        } catch (error) {
            console.error('Erro ao carregar gastos:', error);
            showNotification('Erro ao carregar gastos', 'error');
        }
    }
    
    function updateTotalExpenses() {
        const total = expenses.reduce((sum, expense) => sum + expense.amount, 0);
        totalExpenses.textContent = `R$ ${total.toFixed(2)}`;
        
        // Animação do total
        totalExpenses.style.animation = 'none';
        totalExpenses.offsetHeight; // Trigger reflow
        totalExpenses.style.animation = 'slideDown 0.5s ease';
    }
    
    function filterExpenses() {
        const searchTerm = searchInput.value.toLowerCase();
        const selectedCategory = filterCategory.value;
        
        const filteredExpenses = expenses.filter(expense => {
            const matchesSearch = expense.description.toLowerCase().includes(searchTerm);
            const matchesCategory = !selectedCategory || expense.category === selectedCategory;
            return matchesSearch && matchesCategory;
        });
        
        displayExpenses(filteredExpenses);
    }
    
    function displayExpenses(expensesToShow) {
        const expensesList = document.getElementById('expensesList');
        expensesList.innerHTML = '';
        
        if (!expensesToShow || expensesToShow.length === 0) {
            expensesList.innerHTML = `
                <div class="no-expenses">
                    <i class="fas fa-inbox fa-3x"></i>
                    <p>Nenhum gasto encontrado</p>
                </div>
            `;
            return;
        }
        
        expensesToShow.forEach((expense, index) => {
            const expenseElement = document.createElement('div');
            expenseElement.className = 'expense-item';
            expenseElement.style.animationDelay = `${index * 0.1}s`;
            
            expenseElement.innerHTML = `
                <div class="expense-info">
                    <span class="expense-description">${expense.description}</span>
                    <span class="expense-category">${expense.category}</span>
                </div>
                <span class="expense-amount">R$ ${expense.amount.toFixed(2)}</span>
            `;
            
            expensesList.appendChild(expenseElement);
        });
    }
    
    async function handleSubmit(event) {
        event.preventDefault();
        
        const expense = {
            description: document.getElementById('description').value,
            amount: parseFloat(document.getElementById('amount').value),
            category: document.getElementById('category').value,
            date: new Date().toISOString()
        };
        
        try {
            const response = await fetch(`${API_URL}/expenses`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${localStorage.getItem('token')}`
                },
                body: JSON.stringify(expense)
            });
            
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            // Limpar formulário
            event.target.reset();
            
            // Recarregar gastos
            await loadExpenses();
            
            // Feedback visual
            showNotification('Gasto adicionado com sucesso!', 'success');
        } catch (error) {
            console.error('Erro ao salvar gasto:', error);
            showNotification('Erro ao salvar o gasto', 'error');
        }
    }
    
    function showNotification(message, type = 'success') {
        const notification = document.createElement('div');
        notification.className = `notification ${type}`;
        notification.innerHTML = `
            <i class="fas fa-${type === 'success' ? 'check-circle' : 'exclamation-circle'}"></i>
            ${message}
        `;
        
        document.body.appendChild(notification);
        
        // Animar entrada
        setTimeout(() => notification.classList.add('show'), 100);
        
        // Remover após 3 segundos
        setTimeout(() => {
            notification.classList.remove('show');
            setTimeout(() => notification.remove(), 300);
        }, 3000);
    }
    
    function updateExpensesList() {
        displayExpenses(expenses);
    }
}); 