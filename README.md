# TinyURL Service

TinyURL is a service where users can create short links, such as http://localhost:5226/3rp36a3s, redirecting to longer links, such as https://www.adroit-tt.com.

The application includes:

### Client
A small web UI written in React to interact with the backend URL service

### API
The backend URL service written in C# to manage the tiny URLs

The API has no storage layer, data is stored in an in-memory Dictionary. `ConcurrentDictionary` was used to ensure that concurrent requests can be handled in a thread-safe way. This also allows for fast reads from the dictionary that are not blocked by concurrent write request, which is important for ensuring low latency in the short URL -> long URL lookup

## Running

### Requirements
- node.js
- npm
- dotnet

###

### Run api

```bash
cd api
dotnet run
```

### Run client

```bash
cd client
npm install
npm run dev
```

Then you can open the client at http://localhost:5173/

## Architecture

This application is a proof of concept. The following features would be required for a full production application:

- **Domain**
  -   The tiny URLs have a base URL of `http://localhost:5226/` for demo purposes. In a real application, we would of course use a real domain like `https://tinyurl.com`.
- **Storage**
  -   In a real application, we would want persistent, scalable, and highly available storage, like a distributed database (e.g., Cassandra, DynamoDB) or Redis/Postgres with appropriate replication and persistence strategies.
- **High Availability & Scalability**
  -   **API Servers:** To ensure low latency reads and handle increased traffic, we would deploy multiple API servers behind a load balancer. This allows for horizontal scaling.
  -   **Database:** The chosen storage solution should support replication (e.g., read replicas) to ensure data availability and distribute read load.
  -   **Short Code Uniqueness:** Generating unique short codes across multiple instances requires a robust strategy. Options include:
      -   A dedicated counter service with atomic operations.
      -   Pre-generating a pool of unique codes that instances can claim.
      -   Using a distributed unique ID generator (e.g., Snowflake-style).
- **Rate Limiting**
  -   Implement rate limiting to protect the service from abuse (e.g., preventing users from creating an excessive number of URLs in a short period).
