import globals from "globals";
import eslint from "@eslint/js";
import tseslint from "typescript-eslint";
import pluginVue from "eslint-plugin-vue";
import pluginPrettier from "eslint-plugin-prettier";
import pluginSecurity from "eslint-plugin-security";
import pluginUnicorn from "eslint-plugin-unicorn";
import pluginPromise from "eslint-plugin-promise";
import pluginEslintComments from "eslint-plugin-eslint-comments";

export default [
  eslint.configs.recommended,
  ...tseslint.configs.recommended,
  ...pluginVue.configs["flat/recommended"],
  {
    files: ["src/**/*.{js,mjs,cjs,ts,vue}"], // Include all relevant files in src
    languageOptions: {
      globals: globals.browser,
      parserOptions: {
        parser: tseslint.parser, // Apply TypeScript parser
        ecmaVersion: 2021,
        sourceType: "module",
      },
    },
    plugins: {
      vue: pluginVue,
      prettier: pluginPrettier,
      promise: pluginPromise,
      unicorn: pluginUnicorn,
      security: pluginSecurity,
      "eslint-comments": pluginEslintComments,
    },
    rules: {
      "prettier/prettier": ["error", { tabWidth: 2, endOfLine: "crlf" }],
      indent: ["error", 2],
      "vue/multi-word-component-names": "off",
      "unicorn/prevent-abbreviations": "error",
      "promise/always-return": "warn",
      "security/detect-object-injection": "off",
      "no-console": process.env.NODE_ENV === "production" ? "warn" : "off",
    },
  },
  {
    files: ["**/*.test.js", "**/*.spec.js"], // Test files
    env: {
      jest: true,
    },
  },
];

