(function (window) {
    function triggerCallback(e, callback) {
        if (!callback || typeof callback !== 'function') {
            return;
        }
        var files;
        if (e.dataTransfer) {
            files = e.dataTransfer.files;
        } else if (e.target) {
            files = e.target.files;
        }
        callback.call(null, files);
    }
    function makeDroppable(ele, callback) {
        var input = document.createElement('input');
        input.setAttribute('type', 'file');
        input.setAttribute('id', 'upload');
        input.setAttribute('multiple', false);
        input.style.display = 'none';
        input.addEventListener('change', function (e) {
            triggerCallback(e, callback);
        });
        ele.appendChild(input);

        ele.addEventListener('dragover', function (e) {
            e.preventDefault();
            e.stopPropagation();
            ele.classList.add('dragover');
        });

        ele.addEventListener('dragleave', function (e) {
            e.preventDefault();
            e.stopPropagation();
            ele.classList.remove('dragover');
        });

        ele.addEventListener('drop', function (e) {
            e.preventDefault();
            e.stopPropagation();
            ele.classList.remove('dragover');
            triggerCallback(e, callback);
        });

        ele.addEventListener('click', function () {
            input.value = null;
            input.click();
        });

        ele.addEventListener('change', function () {
            //console.log(input.value)
            readFile();
            $('.modal-footer').css('display', 'block');
        });
    }
    window.makeDroppable = makeDroppable;
})(this);
(function (window) {
    makeDroppable(window.document.querySelector('.PI-droppable'), function (files) {
        //console.log(files);
    });
})(this);