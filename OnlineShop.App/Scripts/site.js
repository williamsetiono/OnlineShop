var siteModal = {
    message: onMessage
};

function onMessage(header, body) {
    var $modal = $('#msgModal');
    $modal.find('#modal-header').html(header);
    $modal.find('#modal-body').html(body);
    $modal.modal();
}