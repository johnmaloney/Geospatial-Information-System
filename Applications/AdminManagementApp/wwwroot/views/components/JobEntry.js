
let submitJobEntry = async () => {

    var form = document.getElementById("jobEntryForm");
    var selector = document.getElementById("sessionSelect");
    var session = selector.options[selector.selectedIndex].value;
    var fileInput = document.querySelector('#file-input-field input[type=file]');

    var reader = new FileReader();
    reader.readAsArrayBuffer(fileInput.files[0]);
    reader.onload = async function () {

        // First upload the file and store it //
        let bytes = Array.from(new Uint8Array(reader.result));

        //if you want the base64encoded file you would use the below line:
        let base64StringFile = btoa(bytes.map((item) => String.fromCharCode(item)).join(""));

        var json = {

            message: form[0].value,
            jobType: form[2].value,
            fileName: fileInput.files[0].name,
            fileContent: base64StringFile, 
            sessionId: session
        };

        const options = {
            method: 'POST',
            body: JSON.stringify(json),
            headers: {
                'Content-Type': 'application/json'
                // 'Content-Type': 'application/x-www-form-urlencoded',
            }
        };
        try {
            const response = await fetch(`api/message`, options);
            const json = await response.json();
            return json;
        } catch (err) {
            console.log('Error submitting job.', err);
        }
    };    
};

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


let JobEntry = {
    render: async () => {


        var session = await getSessionId();

        let view = /*html form*/`
             <form id="jobEntryForm" style="width:100%">
                <div class="field">
                    <label class="label">Select Session to Use</label>
                    <div class="control">
                        <div class="select">
                            <select id="sessionSelect">
                            ${ `<option value=${session.newSessionId}>Start New</option>`}
                            ${ session.directories.map(s =>
                                `<option value=${s.value}>${s.key}</option>`
                            )
                            }
                            </select>
                        </div>
                    </div>
                </div>

                <div class="field">
                    <label class="label">Message</label>
                    <div class="control">
                        <input class="input" type="text" name="message" id="message" placeholder="Text input">
                    </div>
                </div>

                <div class="field">
                    <div class="control">
                        <div class="select">
                            <select>
                              <option value="projectData">Create Projected Data</option>
                              <option value="generateTiles">Create Tiles</option>
                              <option value="analyzeData">Produce Analysis</option>
                            </select>
                        </div>
                    </div>
                </div>

                <div class="field">
                    <div id="file-input-field" class="file has-name">
                      <label class="file-label">
                        <input class="file-input" type="file" name="resume">
                        <span class="file-cta">
                          <span class="file-icon">
                            <i class="fas fa-upload"></i>
                          </span>
                          <span class="file-label">
                            Choose a file…
                          </span>
                        </span>
                        <span class="file-name">
                          ...
                        </span>
                      </label>
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

        const fileInput = document.querySelector('#file-input-field input[type=file]');
        fileInput.onchange = () => {
            if (fileInput.files.length > 0) {
                const fileName = document.querySelector('#file-input-field .file-name');
                fileName.textContent = fileInput.files[0].name;
            }
        };
    }
};

export default JobEntry;