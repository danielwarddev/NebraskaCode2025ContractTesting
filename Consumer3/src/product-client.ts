import axios, { AxiosInstance } from "axios";

export interface Product {
  id: number;
  name: string;
  price: number;
  location: string;
}

export class ProductClient {
  private axiosInstance: AxiosInstance;

  constructor(baseUrl: string) {
    this.axiosInstance = axios.create({
      baseURL: baseUrl,
      headers: {
        "Content-Type": "application/json",
        Accept: "application/json",
      },
    })
  }

  public async getProduct(productId: number): Promise<Product> {
    const response = await this.axiosInstance.get<Product>(`/product/${productId}`);
    return response.data;
  }
}