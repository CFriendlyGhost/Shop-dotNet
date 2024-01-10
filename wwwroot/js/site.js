function addMore(id) {
    var cartCookie = document.cookie
        .split('; ')
        .find(row => row.startsWith('cart='));

    var cartCookieValue = cartCookie.split('=')[1];
    var cartItems = JSON.parse(decodeURIComponent(cartCookieValue));

    cartItems.Items[id]++;

    var updatedCartCookieValue = encodeURIComponent(JSON.stringify(cartItems));
    document.cookie = `cart=${updatedCartCookieValue}; path=/`;
    updateView();
}

function decrease(id) {
    var cartCookie = document.cookie
        .split('; ')
        .find(row => row.startsWith('cart='));

    var cartCookieValue = cartCookie.split('=')[1];
    var cartItems = JSON.parse(decodeURIComponent(cartCookieValue));

    if (cartItems.Items[id] == 1) {
        delete cartItems.Items[id];
    }
    else {
        cartItems.Items[id]--;
    }


    var updatedCartCookieValue = encodeURIComponent(JSON.stringify(cartItems));
    document.cookie = `${updatedCartCookieValue}; path=/`;
    updateView();
}

function deleteItem(id) {
    var cartCookie = document.cookie
        .split('; ')
        .find(row => row.startsWith('cart='));

    var cartCookieValue = cartCookie.split('=')[1];
    var cartItems = JSON.parse(decodeURIComponent(cartCookieValue));

    delete cartItems.Items[id];

    var updatedCartCookieValue = encodeURIComponent(JSON.stringify(cartItems));
    document.cookie = `cart=${updatedCartCookieValue}; path=/`;
    updateView();
}

function updateView() {
    document.location.reload();
}

document.querySelectorAll('.btn-add-more').forEach(function (btn) {
    btn.addEventListener('click', function () {
        var productId = this.getAttribute('id');
        addMore(productId);
    });
});

document.querySelectorAll('.btn-decrease').forEach(function (btn) {
    btn.addEventListener('click', function () {
        var productId = this.getAttribute('id');
        decrease(productId);
    });
});

document.querySelectorAll('.btn-delete').forEach(function (btn) {
    btn.addEventListener('click', function () {
        var productId = this.getAttribute('id');
        deleteItem(productId);
    });
});