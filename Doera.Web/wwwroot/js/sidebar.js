$(function () {
    $(document).on('click', '#sidebarToggle', function (e) {
        e.preventDefault();
        $('#sidebar').toggleClass('d-none');
    });
});