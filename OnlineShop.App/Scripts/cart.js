var CART_QUANTITY = '#cart-quantity';
var COUPON_VALUE = '#CouponValue';
var ORDER_TOTAL = '#cartTotal';
var delay = 1000;
var cart = {
    addItem: onAddItem,
    UpdateQuantityItem: onUpdateQuantityItem,
    IncreaseQuantity: onIncreaseQuantity,
    DecreaseQuantity: onDecreaseQuantity,
    RemoveProduct: onRemoveProduct,
    CheckCouponInput: onCheckCouponInput,
};
// cart: user : 'abc', items : [item1,item2]
// item{ id}
function onAddItem(item, user) {
    spinner.show();
    $.ajax({
        url: '/cart/AddItem',
        type: 'post',
        data: { productId: item },
        success: function (data) {
            message.show(data.Result, data.Message);
            $(CART_QUANTITY).text(data.Amount);
        },
        complete: function () {
            spinner.close();
        }
    });
}

function onUpdateQuantityItem(id, quantity, user) {
    spinner.show();
    $.ajax({
        url: '/cart/UpdateItem',
        type: 'post',
        data: { productId: id,amount:quantity },
        success: function (data) {
            message.show(data.Result, data.Message);
            $(CART_QUANTITY).text(data.Amount);
        },
        complete: function () {
            spinner.close();
        }
    });
}
function removeProduct(id) {
    spinner.show();
    $.ajax({
        url: '/cart/DeleteItem',
        type: 'post',
        data: { productId: id },
        success: function (data) {
            message.show(data.Result, data.Message);
            $(CART_QUANTITY).text(data.Amount);
        },
        complete: function () {
            spinner.close();
        }
    });
}
function onIncreaseQuantity(obj) {
    var $tr = $(obj).closest('tr');
    var id = $tr.data('id');
    var quantity = $tr.find('.cart_quantity_input').val();
    var price = $tr.find('p.cart_price').text();
    quantity = parseInt(quantity) + 1;
    price = parseInt(price);
    $tr.find('.cart_quantity_input').val(quantity);
    $tr.find('.cart_total_price').text(price * quantity);
    onUpdateQuantityItem(id, quantity);
}
function onDecreaseQuantity(obj) {
    var $tr = $(obj).closest('tr');
    var id = $tr.data('id');
    var quantity = $tr.find('.cart_quantity_input').val();
    var price = $tr.find('p.cart_price').text();
    price = parseInt(price);
    if (quantity > 0) {
        quantity = quantity - 1;
        $tr.find('.cart_quantity_input').val(quantity);
        $tr.find('.cart_total_price').text(price * quantity);
        onUpdateQuantityItem(id, quantity);
    }
}
function onRemoveProduct(obj) {
    var $tr = $(obj).closest('tr');
    var id = $tr.data('id');
    $tr.remove();
    removeProduct(id);
}
function onCheckCouponInput(obj) {
    var $this = $(obj);
        setTimeout(function () {
            spinner.show();
            $.ajax({
                url: '/cart/CheckCoupon',
                data: {
                    couponCode:$this.val()
                },
                type:"Post",
                success: function (data) {
                    //if (!data.Result) {
                    //    message.show(data.Result, data.Message);
                    //}
                    $(COUPON_VALUE).text(data.Discount);
                    if (data.Total < 0)
                        data.Total = 0;
                    $(ORDER_TOTAL).text(data.Total);
                },
                complete: function () {
                    spinner.close();
                }
            });
        }, delay);
        
}
