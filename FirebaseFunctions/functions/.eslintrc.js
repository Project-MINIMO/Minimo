module.exports = {
  root: true,
  env: {
    es6: true,
    node: true,
  },
  extends: [
    "eslint:recommended",
    "plugin:import/errors",
    "plugin:import/warnings",
    "plugin:import/typescript",
    "google",
    "plugin:@typescript-eslint/recommended",
  ],
  parser: "@typescript-eslint/parser",
  parserOptions: {
    project: ["tsconfig.json", "tsconfig.dev.json"],
    sourceType: "module",
  },
  ignorePatterns: [
    "/lib/**/*", // Ignore built files.
    "/generated/**/*", // Ignore generated files.
  ],
  plugins: [
    "@typescript-eslint",
    "import",
  ],
  rules: {
    "quotes": ["error", "double"],
    "import/no-unresolved": 0,
    "indent": ["error", 2],
    "max-len": ["warn", { code: 120 }],                // 80 → 120자 허용
    "no-multi-spaces": "off",                          // 주석 앞 공백 허용
    "object-curly-spacing": ["warn", "always"],        // { key: value } 형태 허용
    "@typescript-eslint/no-explicit-any": "off",       // any 허용
    "@typescript-eslint/no-unused-vars": ["warn", { argsIgnorePattern: "^_" }], // 인자는 _로 시작하면 무시
  },
};
