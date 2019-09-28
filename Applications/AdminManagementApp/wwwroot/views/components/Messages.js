

let startHubConnection = async () => {
      

};

let Messages = {
    render: async () => {

        var connection = new signalR.HubConnectionBuilder()
            .withUrl('/ping')
            .build();
        
        // Create a function that the hub can call to broadcast messages.
        connection.on('broadcastMessage', function (message) {

            var encodedMsg = message;
            // Add the message to the page.
            var liElement = document.createElement('li');
            liElement.innerHTML = encodedMsg;
            var parent = document.getElementById('broadcastMessages');
            if (parent.childNodes.length > 0) {
                parent.insertBefore(liElement, parent.childNodes[0]);
            }
            else {
                parent.appendChild(liElement);
            }           
        });

        // Transport fallback functionality is now built into start.
        connection.start()
            .then(function () {
                console.log('connection started');
                document.getElementById('messagePing').addEventListener('click', function (event) {
                    // Call the Send method on the hub.
                    connection.invoke('ping', "Ping from the Admin App");
                });
                
                connection.invoke('ping', "Start up ping...");
            })
            .catch(error => {
                console.error(error.message);
            });
               
        let view =  /*html*/`
            <div class="field"> 
            <section class="field" style="max-height:200px; overflow-y:scroll">
                <ul id="broadcastMessages">
                </ul>
            </section>
            </div>
        `;
        return view;
    }
    , after_render: async () => {
        
    }
};

export default Messages;