import { ProductClient } from "../src/product-client";
import { PactV4, MatchersV3 } from "@pact-foundation/pact";
import path from "path";

describe("product-processor", () => {
  const provider = new PactV4({
    dir: path.join(__dirname, "../pacts"),
    consumer: "Product Processor TS",
    provider: "Product API",
  });

  it("GET /product should return a product", () => {
    const expectedProduct = {
      id: 1,
      name: "Brain Smoother 5000gm",
      price: 100,
      location: "Antarctica",
    };

    return provider
      .addInteraction()
      .given("A product exists")
      .uponReceiving("A request for a product")
      .withRequest("GET", "/products/1", (request) => {
        request.headers({
          Accept: "application/json",
          "Content-Type": "application/json",
        });
      })
      .willRespondWith(200, (response) => {
        response.headers({
          "Content-Type": "application/json",
        });
        response.jsonBody(MatchersV3.like(expectedProduct));
      })
      .executeTest(async (mockServer) => {
        const client = new ProductClient(mockServer.url);
        const response = await client.getProduct(1);
        expect(response).toEqual(expectedProduct);
      });
  });
});
