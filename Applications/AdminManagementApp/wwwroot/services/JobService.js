const JobService = {
    sendJobRequest: async () => {

        var form = document.getElementById("jobForm");
        var FD = new FormData(form);

        const options = {
            method: 'POST', 
            body : FD
        };
        try {
            const response = await fetch(`api/message`, options);
            const json = await response.json();
            // console.log(json)
            return json;
        } catch (err) {
            console.log('Error getting documents', err);
        }

    }
};
export default JobService;