var $image;
var $uploadCrop;

function readFile() {
    var input = $('#upload')[0];
    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            $('.upload-PI').css('display', 'block');
            $('.upload-PI').addClass('ready');
            $uploadCrop.croppie('bind', {
                url: e.target.result
            }).then(function () {
                console.log('jQuery bind complete');
            });

        }

        reader.readAsDataURL(input.files[0]);
    }
    else {
        alert("Sorry - you're browser doesn't support the FileReader API");
    }
}

$uploadCrop = $('#upload-PI').croppie({
    viewport: {
        width: 150,
        height: 150,
        type: 'square'
    },
    boundary: {
        width: 300,
        height: 300
    },
    enableExif: true
});

$('.upload-result').on('click', function (ev) {
    $uploadCrop.croppie('result', {
        type: 'canvas',
        size: 'viewport'
    }).then(function (resp) {
        popupResult({
            src: resp
        });
    });
});

function popupResult(result) {
    $('.user-placeholder').addClass('hidden');
    $('.disimg').attr('src', result.src).removeClass('hidden');
    $('.pp-upload').val(result.src);
    $('#UploadPIModal').modal('hide');
}

$('.remove-image').on('click', function () {
    $('.disimg').addClass('hidden');
    $('.user-placeholder').removeClass('hidden');
    $('.pp-upload').val('');
    $('#UploadPIModal').modal('hide');
});