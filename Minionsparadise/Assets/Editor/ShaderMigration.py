import os
import re

def migrate_shader(file_path):
    with open(file_path, 'r', encoding='utf-8', errors='ignore') as f:
        content = f.read()

    # Targets: GLSLPROGRAM blocks and compiled Program blocks
    targets = [r'GLSLPROGRAM', r'Program\s+"vp"', r'Program\s+"fp"']
    
    modified = False
    new_content = content
    
    for target in targets:
        # Avoid duplicate pragmas if re-running
        pattern = r'([ \t]*)' + target
        
        # Check if pragma already exists before this target
        # We look for #pragma only_renderers followed by optional whitespace and the target
        check_pattern = r'// #pragma only_renderers gles gles3\s+' + target
        if not re.search(check_pattern, new_content):
             new_content = re.sub(pattern, r'\1// #pragma only_renderers gles gles3\n\1' + target.replace('\\s+', ' '), new_content)
             modified = True

    # Cleanup mistakes from previous runs (pragma inside the block)
    cleanup_pattern = r'(\b(GLSLPROGRAM|Program\s+"[v f]p")\s*\{\s*)\s*// #pragma only_renderers gles gles3\n'
    if re.search(cleanup_pattern, new_content):
        new_content = re.sub(cleanup_pattern, r'\1', new_content)
        modified = True
    
    # Specific cleanup for the previous run's mistake
    if '  // #pragma only_renderers gles gles3\n' in new_content:
        new_content = new_content.replace('  // #pragma only_renderers gles gles3\n', '')
        modified = True

    if modified:
        with open(file_path, 'w', encoding='utf-8') as f:
            f.write(new_content)
        return True
    return False

def main():
    shader_dir = r"c:\Users\Pierr\Documents\Projects\Decompiles\Minions_Paradise\SANDBOX\5-3-5p4-UP\Minionsparadise\Assets\Shader"
    for root, dirs, files in os.walk(shader_dir):
        for file in files:
            if file.endswith(".shader"):
                full_path = os.path.join(root, file)
                if migrate_shader(full_path):
                    print(f"Migrated: {file}")

if __name__ == "__main__":
    main()
