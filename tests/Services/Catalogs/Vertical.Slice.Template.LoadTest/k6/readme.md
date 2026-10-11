# Run Test Cases

``` bash
k6 run --exec createProduct --vus 20 --duration 90s script.js

k6 run --exec getProductsByPage --vus 20 --duration 90s script.js

k6 run --exec getProductById --vus 20 --duration 90s --env PRODUCT_ID=<your-product-id> script.js
```