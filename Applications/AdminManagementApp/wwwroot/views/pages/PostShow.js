import Utils from './../../services/Utils.js';

let getPost = async (id) => {
    const options = {
       method: 'GET',
       headers: {
           'Content-Type': 'application/json'
       }
   };
   try {
       const response = await fetch(`api/message/` + id, options)
       const json = await response.json();
       // console.log(json)
       return json;
   } catch (err) {
       console.log('Error getting documents', err);
   }
}

let PostShow = {

    render: async () => {
        let request = Utils.parseRequestURL();
        let post = await getPost(request.id);

        return /*html*/`
            <section class="section">
                <h1> Correlation Id : ${post.correllationId}</h1>
                <p> Id : ${post.id} </p>
                <p> Type : ${post.type} </p>
                <p> Version : ${post.version} </p>
            </section>
        `;
    }
    , after_render: async () => {
    }
};

export default PostShow;