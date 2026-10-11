const string PostgresImage = "postgres";
const string PostgresTag = "17";
const string RabbitMqImage = "rabbitmq";
const string RabbitMqTag = "4-management";

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres").WithImage(PostgresImage).WithImageTag(PostgresTag);
var catalogsDb = postgres.AddDatabase("catalogsdb");
var rabbitMq = builder.AddRabbitMQ("rabbitmq").WithImage(RabbitMqImage).WithImageTag(RabbitMqTag);

builder
    .AddProject<Projects.Vertical_Slice_Template_Api>("catalogs-api")
    .WithReference(catalogsDb)
    .WithReference(rabbitMq)
    .WaitFor(catalogsDb)
    .WaitFor(rabbitMq)
    .WithHttpHealthCheck("/health");

builder.Build().Run();
