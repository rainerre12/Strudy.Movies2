

function initializeSelect2() {

    $('#selectMultipleGenreIds').select2()


    var selectedGenreIds = decodeBase64Genre(encodedSelectedGenreIds);
    if (Array.isArray(selectedGenreIds) && selectedGenreIds.length > 0) {
        $('#selectMultipleGenreIds').val(selectedGenreIds).trigger('change');
    }
}

function UpdateMovie() {
    $('#updateMovie').click(function () {

        var isValid = true;

        if (isValid) {

            var MovieName = $('#inputmoviename').val();
            var Genreid = $('#selectMultipleGenreIds').val();
            var data = {
                Movies: {
                    Id: decodeBase64ToID(encodedID),
                    Name: MovieName
                },
                selectMultipleGenreIds: Genreid.map(Number)

            };

            $.ajax({
                url: '/Home/PostUpdateMovie',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(data),
                success: function (response) {
                    toastr.success('Movie Registered');
                    setTimeout(function () {
                        window.location.href = '/Home/Index'; // Redirect to the same page after a delay
                    }, 2000);
                },
                error: function (xhr, status, error) {
                    //if (xhr.status = 400) {
                    //    alert('Movie with the same name already exists.');
                    var errorMessage = xhr.responseJSON && xhr.responseJSON.message;
                    if (errorMessage) {
                        toastr.error(errorMessage);
                    } else {
                        console.error('Error:', error);
                        toastr.error('Error occurred while registering Movie');
                    }
                }

            });

        }


    });
}


function decodeBase64Genre(encodedString) {
    try {
        var decodedString = atob(encodedString);
        return JSON.parse(decodedString);
    } catch (error) {
        console.error('Error decoding base64 string', error)
        return [];
    }
}

function decodeBase64ToID(base64String) {
    try {
        // Decode the base64 string to a binary string
        var binaryString = atob(base64String);

        // Convert the binary string to a byte array
        var byteArray = new Uint8Array(binaryString.length);
        for (var i = 0; i < binaryString.length; i++) {
            byteArray[i] = binaryString.charCodeAt(i);
        }

        // Convert the byte array to an integer ID
        var id = new DataView(byteArray.buffer).getInt32(0, true);

        return id;
    } catch (error) {
        console.error('Error decoding base64 ID:', error);
        return null; // Return a default value or handle the error as needed
    }
}


$(document).ready(function () {
    initializeSelect2();
    decodeBase64Genre();
    decodeBase64ToID();
    UpdateMovie();
});