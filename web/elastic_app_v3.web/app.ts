////want to use ESModule to import DTOs later
//interface SignUpRequest {
//    firstName: string;
//    lastName: string;
//    userName: string;
//    password: string;
//    reEnteredPassword: string;
//}
//interface SignUpResponse {
//    id: Guid;
//}
//interface ApiError { //exceptions return ProblemDetails and errors return ApiError, I think we need a standard error response or will break consumer
//    code: string;
//    message: string;
//}

//const form = document.getElementById("signup-form") as HTMLFormElement;
//const messageEl = document.getElementById("message") as HTMLParagraphElement;

//form.addEventListener("submit", async (event) => {
//    event.preventDefault(); // prevent default form submission, what is default behaviour?

//    // Build the request object from input fields
//    const request: SignUpRequest = {
//        firstName: (document.getElementById("firstName") as HTMLInputElement).value, //why alias everything?
//        lastName: (document.getElementById("lastName") as HTMLInputElement).value,
//        userName: (document.getElementById("userName") as HTMLInputElement).value,
//        password: (document.getElementById("password") as HTMLInputElement).value,
//        reEnteredPassword: (document.getElementById("reEnteredPassword") as HTMLInputElement).value
//    };

//    try {
//        const response = await fetch("http://localhost:8081/elastic-app/v1/signup", {
//            method: "POST",
//            headers: {
//                "Content-Type": "application/json"
//            },
//            body: JSON.stringify(request)
//        });

//        if (!response.ok) {
//            const error: ApiError = await response.json();
//            messageEl.textContent = `Error: ${error.message}`;
//            messageEl.style.color = "red";
//            return;
//        }

//        const data: SignUpResponse = await response.json(); //what would client do with the user Id? 

//        form.reset(); //why optional? What does this do?
//    } catch (err) {
//        messageEl.textContent = "Network error. Please try again."; //why do all this on messageEl?
//        messageEl.style.color = "red";
//        console.error(err);
//    }
//});