var dojoConfig;
(function () {
    var baseUrl = 'http://localhost:8080/assets/';//location.pathname.replace(/\/[^/]+$/, '/../assets/');
    dojoConfig = {
        async: 1,
        // Load dgrid and its dependencies from a local copy.
        // If we were loading everything locally, this would not
        // be necessary, since Dojo would automatically pick up
        // dgrid as a sibling of the dojo folder.
        packages: [
            { name: 'dgrid', location: 'http://localhost:8080/assets/dgrid' }
        ]
    };
}());