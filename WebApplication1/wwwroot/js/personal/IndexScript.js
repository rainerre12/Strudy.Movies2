

var _urlPersons;
var _urlMovies;
var _urlAssigned;
var _urlUpdateMovies

function initScript(urlPersons, urlMovies, urlAssigned,urlUpdateMovie) {
    _urlMovies = urlMovies;
    _urlPersons = urlPersons;
    _urlAssigned = urlAssigned;
    _urlUpdateMovies = urlUpdateMovie;
    
}

$(function () {
    $('.select2').each(function () {
        $(this).select2();
    });

    $("#movietbl").DataTable({
        // "responsive": true, "lengthChange": false, "autoWidth": false,
        // "buttons": ["copy", "csv", "excel", "pdf", "print", "colvis"]
        "responsive": false, "lengthChange": true, "autoWidth": true
    }).buttons().container().appendTo('#movietbl_wrapper .col-md-6:eq(0)');

    $("#userstbl").DataTable({
        "responsive": false, "lengthChange": true, "autoWidth": true
    }).buttons().container().appendTo('#userstbl_wrapper .col-md-6:eq(0)');

    $('#modalRegister').on('show.bs.modal', function (event) {
        console.log('Sulod modal')
        var button = $(event.relatedTarget);
        var modalType = button.data('modal-type');

        switch (modalType) {

            case 'RegisterUser':

                //console.log('RegisterUser');
                fetch(_urlPersons)
                    .then(response => response.text())
                    .then(data => {
                        $('#ModalContent').html(data);
                    })
                    .catch(error => console.error('Error:', error));
                break;
            case 'RegisterMovie':

                //console.log('RegisterMovie');
                fetch(_urlMovies)
                    .then(response => response.text())
                    .then(data => {
                        $('#ModalContent').html(data);
                    })
                    .catch(error => console.error('Error:', error));
                break;
            case 'AssignUser':

                fetch(_urlAssigned)
                    .then(response => response.text())
                    .then(data => {
                        $('#ModalContent').html(data);
                    })
                    .catch(error => console.error('Error:', error));
                break;
            case 'UpdateMovie':
                var movieId = button.attr('data-id');
                console.log(movieId);
                fetch(_urlUpdateMovies + '/' + movieId)
                    .then(response => response.text())
                    .then(data => {
                        $('#ModalContent').html(data);
                    })
                    .catch(error => console.error('Error:', error))
                break;
            default:
                console.log('Error');
                break;
        }
    })


});
