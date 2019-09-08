
let submitLogEntry = async () => {

    var form = document.getElementById("logEntryForm");
    var fData = new FormData(form);

    const options = {
        method: 'POST',
        body: fData
    };
    try {
        const response = await fetch(`api/log`, options);
        const json = await response.json();
        return json;
    } catch (err) {
        console.log('Error submitting log.', err);
    }
};

let LogEntry = {
    render: async () => {
        let view = /*html form*/`
            <form id = "logEntryForm">
                <div class="field">
                    <label class="label">Enter Log Details</label>
                    <div class="control">
                        <input class="input" type="text" name="message" placeholder="Text input">
                    </div>
                </div>

                <div class="field is-grouped">
                    <div class="control">
                        <button class="button is-link">Submit</button>
                    </div>
                    <div class="control">
                        <button class="button is-text">Cancel</button>
                    </div>
                </div>
            </form>
        `;
        return view;
    }
    , after_render: async () => {
        var form = document.getElementById("logEntryForm");
        form.addEventListener("submit", function (event) {
            event.preventDefault();

            submitLogEntry();
        });
    }
};

export default LogEntry;