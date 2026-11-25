FROM dpage/pgadmin4:latest

USER root

COPY generate_config.py /generate_config.py

ENTRYPOINT ["python3", "/generate_config.py"]