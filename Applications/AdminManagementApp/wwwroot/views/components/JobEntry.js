
let submitJobEntry = async () => {

    var form = document.getElementById("jobEntryForm");
    var fData = new FormData(form);

    const options = {
        method: 'POST',
        body: fData
    };
    try {
        const response = await fetch(`api/message`, options);
        const json = await response.json();
        return json;
    } catch (err) {
        console.log('Error submitting job.', err);
    }
};

let JobEntry = {
    render: async () => {
        let view = /*html form*/`
            <form id="jobEntryForm">
                <div class="field">
                    <label class="label">Message</label>
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
        var form = document.getElementById("jobEntryForm");
        form.addEventListener("submit", function (event) {
            event.preventDefault();

            submitJobEntry();
        });
    }
};

export default JobEntry;