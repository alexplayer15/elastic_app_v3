import { build } from 'esbuild';

//to do: make sure dist is refreshed properly on build
await build({
  entryPoints: ['src/application/identity/signup.ts', 'src/application/identity/login.ts'],
  bundle: true,
  outdir: 'dist',
  sourcemap: true,
  target: 'es2020',
  format: 'esm',
  platform: 'browser',
});