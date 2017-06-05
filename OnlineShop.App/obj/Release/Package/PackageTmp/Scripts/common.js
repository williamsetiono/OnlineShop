var notify = '<div class="alert alert-dismissable">' +
    '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
    '</div>';
var img = '<img src="#" alt=""/>';
var imgContainer = '<div class="img-item">' +
    //'<div class = "img-control">' +
    //'<i class="fa-check fa fa-2x text-primary border-right-1 upgrade " onClick ="onImgUpgrade(this)"></i>' +
    //'<i class="fa-ban fa fa-2x text-danger cancel" onClick ="onImgCancel(this)"></i>' +
    //'</div>' +
    '</div>';
var spinner = {
    show: spinnerShow,
    close: spinnerClose
};
var message = {
    show: onMessageShow,
    close: onMessageClose,
};
//var imgCatalog = {
//    itemClick: onItemClick
//};
function onItemClick() {
    var $this = $(this);
}

function readURL(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            var $img = $(img);
            $img.attr('src', e.target.result);
            $('.img-preview').html($img);
        };
        reader.readAsDataURL(input.files[0]);
        $('#mainImg').val(0);
        if (input.files.length > 0) {
            
            $.each(input.files, function(i, o) {
                var subfile = new FileReader();
                subfile.onload = function (e) {
                    var $img = $(img);
                    var $imgwrapper = $('.img-catalog');
                    var $imgItem = $(imgContainer);
                    $imgItem.data('id', $imgwrapper.find('.img-item').length);
                    $imgItem.append($img);
                    $img.attr('src', e.target.result).data('index',i);
                    $imgwrapper.append($imgItem);
                };
                subfile.readAsDataURL(input.files[i]);
            });
        }
    }
}
function getExtension(fileName) {
    return fileName.split('.').pop();
}
function getFileName(fileName) {
    return fileName.substr(0, fileName.indexOf(getExtension(fileName))-1);
}
function onImgCancel(obj) {
    var $this = $(obj);
    var $currentImg = $this.closest('.img-item').find('img');
    var id = $currentImg.data('id');
    $('#Images').files[id].remove();
    $this.closest('.img-item').remove();
}
function onImgUpgrade(obj) {
    var $this = $(obj);
    var $preview = $this.closest('.img-wrapper').find('.img-preview').find('img');
    var $currentImg = $this.closest('.img-item').find('img');
    $('#mainImg').val($currentImg.data('id'));
    $preview.attr('src', $currentImg.attr('src'));
}
function convertDate(value) {
    return new Date(parseInt(value.replace("/Date(", "").replace(")/", ""), 10)).toLocaleDateString();
}

function showNotify(status, message) {
    var $notify = $(notify);
    if (status == true) {
        $notify.addClass('alert-success');
    } else {
        $notify.addClass('alert-danger');
    }
    $notify.append(message);
    $('#notify-wrapper').append($notify);
}

function spinnerShow() {
    $('#loadding-wrapper').modal('show');
}

function spinnerClose() {
    $('#loadding-wrapper').modal('toggle');
}

function onMessageShow(result,message) {
    var alertClass = "alert-danger";
    if (result == true) alertClass = "alert-success";
    var $notifyMessage = $('#notify-message');
    var $notify = $(notify);
    $notify.attr('id', 'alert-wrapper');
    $notify.addClass(alertClass);
    $notify.append(message);
    $notifyMessage.append($notify);
    $notifyMessage.show(600);
    setTimeout(onMessageClose,3000);
}
function onMessageClose() {
    $('#notify-message').find('button').click();
    $('#notify-message').hide();
}