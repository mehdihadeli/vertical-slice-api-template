import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
  vus: 10, // Virtual Users
  duration: '120s', // Test Duration
};

const BASE_URL = 'http://localhost:4000';

// Headers for requests
const HEADERS = {
  'Content-Type': 'application/json',
  'X-API-KEY': '12345-abcdef', // Replace with your actual API Key
};

// Test Data
const testProduct = {
  name: 'Sample Product',
  categoryId: '2FB8C370-4DEC-496F-9C3F-DF788830F0A5',
  price: 99.99,
  description: 'A sample product for testing',
};

export function createProduct() {
  const createRes = http.post(`${BASE_URL}/api/v1/products`, JSON.stringify(testProduct), { headers: HEADERS });
  check(createRes, {
    'create product status is 201': (r) => r.status === 201,
  });
  console.log(`Created product with ID: ${createRes.json('id')}`);
  return createRes.json('id');
}

export function getProductsByPage() {
  const getPageRes = http.get(`${BASE_URL}/api/v1/products?PageSize=10&PageNumber=1`, { headers: HEADERS });
  check(getPageRes, {
    'get products status is 200': (r) => r.status === 200,
  });
  console.log('Fetched products by page');
}

export function getProductById() {
  const productId = __ENV.PRODUCT_ID; // Use PRODUCT_ID from environment variable
  if (!productId) {
    throw new Error('PRODUCT_ID environment variable is required');
  }
  const getByIdRes = http.get(`${BASE_URL}/api/v1/products/${productId}`, { headers: HEADERS });
  check(getByIdRes, {
    'get product by id status is 200': (r) => r.status === 200,
  });
  console.log(`Fetched product with ID: ${productId}`);
}

export default function () {
  // Default test flow
  const productId = createProduct();
  getProductsByPage();
  if (productId) {
    getProductById({ productId });
  }
  sleep(1); // Simulate time between requests
}
