
let submitJobEntry = async () => {

    var form = document.getElementById("jobEntryForm");

    var json = {
        message : form[0].value,
        jobType: form[1].value,
        fileName : form[2].value
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

let JobEntry = {
    render: async () => {
        let view = /*html form*/`
             <form id="jobEntryForm">
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
        }
    }
};

export default JobEntry;