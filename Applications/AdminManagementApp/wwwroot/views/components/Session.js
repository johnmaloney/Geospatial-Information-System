
let getNewSessionId = async () => {

    var form = document.getElementById("sessionIdForm");
    var fData = new FormData(form);
}

let getSessionId = async () => {
      
    const options = {
        method: 'GET'
    };
    try {
        const response = await fetch(`api/session`, options);
        return await response.json();
    } catch (err) {
        console.log('Error submitting log.', err);
    }
};

let Session = {
    render: async () => {


        var session = await getSessionId();
        //document.getElementById("sessionId").value = id;

        let view = /*html form*/`
            <form id ="sessionIdForm">               
                <div class="field">
                    <label class="label">Select Session to Use</label>
                    <div class="control">
                        <div class="select">
                            <select id="sessionSelect">
                            ${ `<option value=${session.newSessionId}">Start New</option>` }
                            ${ session.directories.map(s => 
                                `<option value=${s.value}">${s.key}</option>`
                            )
                            }
                            </select>
                        </div>
                    </div>
                </div>
                <div class="field is-grouped">
                    <div class="control">
                        <button class="button is-link">Get a New Id</button>
                    </div>
                </div>
            </form>
        `;
        return view;
    }
    , after_render: async () => {
        
        await getSessionId();

        var form = document.getElementById("sessionIdForm");
        form.addEventListener("submit", function (event) {
            event.preventDefault();

        });
    }
};

export default Session;