document.addEventListener('DOMContentLoaded', function() {
    // Attach listeners to quantity inputs and remove buttons
    function attachCartListeners() {
        document.querySelectorAll('.cart-qty-input').forEach(function(input) {
            input.removeEventListener('change', onQtyChange); // Prevent duplicate
            input.addEventListener('change', onQtyChange);
        });
        document.querySelectorAll('.cart-remove-btn').forEach(function(btn) {
            btn.removeEventListener('click', onRemoveClick); // Prevent duplicate
            btn.addEventListener('click', onRemoveClick);
        });
        // Checkout button submits form to Checkout
        var checkoutBtn = document.getElementById('checkoutBtn');
        if (checkoutBtn) {
            checkoutBtn.onclick = function() {
                var form = document.getElementById('cartForm');
                form.action = '/Pharmacy/Pharmacy/Checkout';
                form.method = 'post';
                form.submit();
            };
        }
    }

    // Handle quantity change
    function onQtyChange(e) {
        var form = document.getElementById('cartForm');
        var data = new FormData(form);
        fetch('/Pharmacy/Pharmacy/UpdateCart', {
            method: 'POST',
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'RequestVerificationToken': document.querySelector('input[name=__RequestVerificationToken]').value
            },
            body: new URLSearchParams([...data])
        })
        .then(response => response.text())
        .then(html => {
            document.getElementById('cart-table-container').innerHTML = html;
            attachCartListeners();
        });
    }

    // Handle remove button
    function onRemoveClick(e) {
        var idx = this.getAttribute('data-index');
        var form = document.getElementById('cartForm');
        var data = new FormData(form);
        data.append('removeIndex', idx);
        fetch('/Pharmacy/Pharmacy/UpdateCart', {
            method: 'POST',
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'RequestVerificationToken': document.querySelector('input[name=__RequestVerificationToken]').value
            },
            body: new URLSearchParams([...data])
        })
        .then(response => response.text())
        .then(html => {
            document.getElementById('cart-table-container').innerHTML = html;
            attachCartListeners();
        });
    }

    attachCartListeners();
});
