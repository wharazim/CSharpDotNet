Demo based on OWIN Middleware.

Replaces dependance on IIS stack (HttpHandler + HttpModules). Injects itself in the pipline between the host (IIS)
and WebAPI and allows manipulation of the request/response stack using a Dictionary of <string, object>.
Allows the WebAPI to access low-level service items that are not accessible in the HttpHandler (Attribute filters).

Middleware is where you check the incoming request for an authorization header. 
Call the AS (Authentication Server) and get the user/client identity and scopes (claims) for the request.
Use Authentication and Authorization filters as before based on the pricipal created in the middleware.

Also replaces client-side Message Handler
public class MyHandler : DelegatingHandler where we override the SendAync task to do our own manipultion of the reuqest 
(add hmac hash, etc).


Reference: Pluralsight - Web API v2 Security

